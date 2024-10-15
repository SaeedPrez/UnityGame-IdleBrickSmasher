using System;
using Core;
using Data;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using VInspector;

namespace Menus
{
    public class BallMenuRow : MonoBehaviour
    {
        [Tab("General")]
        [SerializeField] private GameObject _lockedUi;
        [SerializeField] private TMP_Text _lockLevelUi;
        [SerializeField] private Button _talentsButton;
        [SerializeField] private TMP_Text _ballNrUi;

        [Tab("Stats")] 
        [SerializeField] private TMP_Text _speedStatsUi;
        [SerializeField] private TMP_Text _dmgStatsUi;
        [SerializeField] private TMP_Text _dpsStatsUi;
        [SerializeField] private TMP_Text _critHitStatsUi;
        [SerializeField] private TMP_Text _critDmgStatsUi;
        [SerializeField] private TMP_Text _activeHitsStatsUi;
        [SerializeField] private TMP_Text _otherStatsUi;
        
        public Ball Ball { get; private set; }
        public BallData Data { get; private set; }
        public bool IsUnlocked { get; private set; }

        private bool _isTalentsVisible;

        private void OnEnable()
        {
            _talentsButton.onClick.AddListener(OnTalentButtonClicked);
            
            if (!IsUnlocked)
                _lockedUi.SetActive(true);
        }

        private void OnDisable()
        {
            _talentsButton.onClick.RemoveListener(OnTalentButtonClicked);

        }

        private void OnTalentButtonClicked()
        {
            ToggleTalentsWindow();
        }

        public void SetBall(Ball ball)
        {
            Ball = ball;
        }
        
        public void SetData(BallData data)
        {
            Data = data;
            Ball.SetData(data);
            
            _ballNrUi.SetText($"Ball {Ball.Data.Id}");
            UpdateLockLevelUi();
        }

        public void Unlock()
        {
            _lockedUi.SetActive(false);
            IsUnlocked = true;
            UpdateStatsUi();
            EventManager.I.TriggerBallMenuRowUnlocked(this);
        }

        private void UpdateLockLevelUi()
        {
            _lockLevelUi.SetText($"Unlocks at level {Data.UnlockLevel}");
        }

        private void ToggleTalentsWindow()
        {
            var rect = GetComponent<RectTransform>();

            if (_isTalentsVisible)
            {
                rect.DOSizeDelta(new Vector2(rect.sizeDelta.x, rect.sizeDelta.y / 3f), 0.5f).OnComplete(() =>
                {
                    var x = GetComponentInParent<VerticalLayoutGroup>();
                    x.SetLayoutVertical();
                });
                _isTalentsVisible = false;
            }
            else
            {
                rect.DOSizeDelta(new Vector2(rect.sizeDelta.x, rect.sizeDelta.y * 3f), 0.5f).OnComplete(() =>
                {
                    var x = GetComponentInParent<VerticalLayoutGroup>();
                    x.SetLayoutVertical();
                });
                _isTalentsVisible = true;
            }
        }
        
        private void UpdateStatsUi()
        {
            _speedStatsUi.SetText(Helper.GetNumberAsString(GameManager.Data.GetBallSpeed(Ball)));
            _dmgStatsUi.SetText(Helper.GetNumberAsString(GameManager.Data.GetBallDamage(Ball)));
            _activeHitsStatsUi.SetText(GameManager.Data.GetActivePlayHits(Ball).ToString());
        }
    }
}