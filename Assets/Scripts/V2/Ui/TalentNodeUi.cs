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
            if (Data.Level >= MaxLevel)
                return;

            Data.Level++;
            
            _event.TriggerTalentPointLeveledUp(this);
        }

        #endregion

        #region Lock

        private void Lock()
        {
            _button.gameObject.SetActive(false);
            _line.gameObject.SetActive(false);
        }
        
        private void Unlock()
        {
            if (Data.Unlocked)
                return;

            Data.Unlocked = true;
            _button.gameObject.SetActive(true);
            _line.gameObject.SetActive(true);

            if (ParentNodeUi != null)
            {
                _line.gameObject.SetActive(true);
                SetupLine();
            }
        }

        #endregion

        #region Line

        private void SetupLine()
        {
            var point1 = Vector2.zero;
            var point2 = ((RectTransform)ParentNodeUi.transform).anchoredPosition - ((RectTransform)transform).anchoredPosition;;

            if (point2.x < point1.x)
                point2.x += _size.x * 0.5f;
            else if (point2.x > point1.x)
                point2.x -= _size.x * 0.5f;

            if (point2.y < point1.y)
                point2.y += _size.y * 0.5f;
            else if (point2.y > point1.y)
                point2.y -= _size.y * 0.5f;

            _line.Points[0] = point1;
            _line.Points[1] = point2;

            _line.SetAllDirty();
        }

        #endregion
    }
}