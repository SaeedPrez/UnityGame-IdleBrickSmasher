using System;
using System.Collections;
using Core;
using Data;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
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
        [SerializeField] private float _rectSizeOpenY;

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
        private RectTransform _rect;
        private Vector2 _rectStartSize;

        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
            _rectStartSize = _rect.sizeDelta;
        }

        private void OnEnable()
        {
            EventManager.I.OnBottomMenuHidden += OnBottomMenuHidden;
            _talentsButton.onClick.AddListener(OnTalentButtonClicked);
            
            if (!IsUnlocked)
                _lockedUi.SetActive(true);
            
        }
        
        private void OnDisable()
        {
            EventManager.I.OnBottomMenuHidden -= OnBottomMenuHidden;
            _talentsButton.onClick.RemoveListener(OnTalentButtonClicked);

        }

        private void OnBottomMenuHidden(MenuBase menu)
        {
            if (menu is BallsMenu)
                HideTalentWindow();
        }

        private void OnTalentButtonClicked()
        {
            ToggleTalentWindow();
        }

        /// <summary>
        /// Sets the related ball.
        /// </summary>
        /// <param name="ball"></param>
        public void SetBall(Ball ball)
        {
            Ball = ball;
        }
        
        /// <summary>
        /// Sets the related ball data.
        /// </summary>
        /// <param name="data"></param>
        public void SetData(BallData data)
        {
            Data = data;
            Ball.SetData(data);
            
            _ballNrUi.SetText($"Ball {Ball.Data.Id}");
            UpdateLockLevelUi();
        }

        /// <summary>
        /// Unlocks ball.
        /// </summary>
        public void Unlock()
        {
            _lockedUi.SetActive(false);
            IsUnlocked = true;
            UpdateStatsUi();
            EventManager.I.TriggerBallMenuRowUnlocked(this);
        }

        /// <summary>
        /// Toggles the talent window.
        /// </summary>
        private void ToggleTalentWindow()
        {
            var targetSize = _isTalentsVisible
                ? _rectStartSize
                : new Vector2(_rectStartSize.x, _rectSizeOpenY);

            _isTalentsVisible = !_isTalentsVisible;

            var tween = _rect.DOSizeDelta(targetSize, 0.25f)
                .SetEase(Ease.InOutCirc);

            LayoutRebuilder.MarkLayoutForRebuild(_rect);
        }

        /// <summary>
        /// Closes the talent window.
        /// </summary>
        public void HideTalentWindow()
        {
            _isTalentsVisible = false;
            _rect.sizeDelta = _rectStartSize;
        }
        
        /// <summary>
        /// Updates the unlock UI.
        /// </summary>
        private void UpdateLockLevelUi()
        {
            _lockLevelUi.SetText($"Unlocks at level {Data.UnlockLevel}");
        }
        
        /// <summary>
        /// Updates the stats UI.
        /// </summary>
        private void UpdateStatsUi()
        {
            _speedStatsUi.SetText(Helper.GetNumberAsString(GameManager.Data.GetBallSpeed(Ball)));
            _dmgStatsUi.SetText(Helper.GetNumberAsString(GameManager.Data.GetBallDamage(Ball)));
            _activeHitsStatsUi.SetText(GameManager.Data.GetActivePlayHits(Ball).ToString());
        }
    }
}