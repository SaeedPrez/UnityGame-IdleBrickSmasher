using System.Collections;
using DG.Tweening;
using Prez.V2.Data.Save;
using Prez.V2.Managers;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace Prez.V2.Ui
{
    public class TalentNodeUi : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private UILineRenderer _line;
        [SerializeField] private Vector2 _size;
        
        [field:SerializeField] public string Name { get; private set; }
        [field:SerializeField] public string Description { get; private set; }
        [field:SerializeField] public int MaxLevel { get; private set; }
        [field:SerializeField] public TalentNodeUi ParentNodeUi { get; private set; }
        [field:SerializeField] public int ParentLevel { get; private set; }
        [field:SerializeField] public TalentNodeData Data { get; private set; }

        private EventManager _event;
        private Vector2 _lineParentPosition;
   
        #region Bootstrap

        private void Awake()
        {
            _event = RefManager.Event;
        }

        private void OnEnable()
        {
            _event.OnTalentNodeLeveledUp += OnTalentNodeLeveledUp;
            _button.onClick.AddListener(OnButtonClicked);
        }

        private void OnDisable()
        {
            _event.OnTalentNodeLeveledUp -= OnTalentNodeLeveledUp;
            _button.onClick.RemoveListener(OnButtonClicked);
        }

        private void Start()
        {
            if (Data.Unlocked)
                Unlock();
            else
                Lock();
        }

        #endregion

        #region Observers

        private void OnTalentNodeLeveledUp(TalentNodeUi nodeUi)
        {
            if (ParentNodeUi == nodeUi && nodeUi.Data.Level >= ParentLevel)
                Unlock();
        }

        private void OnButtonClicked()
        {
            LevelUp();
        }

        #endregion

        #region Lock

        /// <summary>
        /// Locks the talent node.
        /// </summary>
        private void Lock()
        {
            _line.gameObject.SetActive(false);
            _button.gameObject.SetActive(false);
        }
        
        /// <summary>
        /// Unlocks the talent node.
        /// </summary>
        private void Unlock()
        {
            if (Data.Unlocked)
                return;

            _button.gameObject.SetActive(true);
            _button.transform.localScale = Vector3.zero;
            _button.transform.DOScale(1, 0.5f)
                .SetEase(Ease.OutBack);

            StartCoroutine(DrawLine());
        }

        #endregion

        #region Line

        /// <summary>
        /// Calculates the line point positions.
        /// </summary>
        private void CalculateLinePoints()
        {
            var point2 = ((RectTransform)ParentNodeUi.transform).anchoredPosition - ((RectTransform)transform).anchoredPosition;

            if (point2.x < 0)
                point2.x += _size.x * 0.5f;
            else if (point2.x > 0)
                point2.x -= _size.x * 0.5f;

            if (point2.y < 0)
                point2.y += _size.y * 0.5f;
            else if (point2.y > 0)
                point2.y -= _size.y * 0.5f;

            _lineParentPosition = point2;
        }

        /// <summary>
        /// Draws a line between node
        /// and the parent node.
        /// </summary>
        /// <returns></returns>
        private IEnumerator DrawLine()
        {
            if (!ParentNodeUi)
                yield break;
            
            CalculateLinePoints();

            _line.Points[0] = _lineParentPosition;
            _line.Points[1] = _lineParentPosition;
            _line.gameObject.SetActive(true);

            DOVirtual.Vector2(_lineParentPosition, Vector2.zero, 0.25f, position =>
            {
                _line.Points[1] = position;
                _line.SetAllDirty();
            }).SetEase(Ease.OutFlash).OnComplete(() => { Data.Unlocked = true; });
        }

        #endregion

        #region Level

        /// <summary>
        /// Levels up node.
        /// </summary>
        private void LevelUp()
        {
            if (Data.Level >= MaxLevel)
                return;

            Data.Level++;
            
            _event.TriggerTalentPointLeveledUp(this);
        }

        #endregion
    }
}