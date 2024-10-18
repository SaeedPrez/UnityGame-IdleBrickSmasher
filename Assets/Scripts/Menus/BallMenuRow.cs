using System.Collections;
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
        [SerializeField] private float _rectSizeOpenY;
        [SerializeField] private CanvasGroup _upgrades;

        [Tab("Stats")] 
        [SerializeField] private TMP_Text _speedStatsUi;
        [SerializeField] private TMP_Text _dmgStatsUi;
        [SerializeField] private TMP_Text _dpsStatsUi;
        [SerializeField] private TMP_Text _critHitStatsUi;
        [SerializeField] private TMP_Text _critDmgStatsUi;
        [SerializeField] private TMP_Text _activeHitsStatsUi;
        [SerializeField] private TMP_Text _otherStatsUi;

        [Tab("Speed")] 
        [SerializeField] private Button _speedUpgradeButton;
        [SerializeField] private TMP_Text _speedUpgradeLevelUi;
        [SerializeField] private TMP_Text _speedUpgradeCurrentUi;
        [SerializeField] private TMP_Text _speedUpgradeNextUi;
        [SerializeField] private TMP_Text _speedUpgradeCostUi;

        [Tab("Dmg")] 
        [SerializeField] private Button _dmgUpgradeButton;
        [SerializeField] private TMP_Text _dmgUpgradeLevelUi;
        [SerializeField] private TMP_Text _dmgUpgradeCurrentUi;
        [SerializeField] private TMP_Text _dmgUpgradeNextUi;
        [SerializeField] private TMP_Text _dmgUpgradeCostUi;
        
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
            EventManager.I.OnCoinsUpdated += OnCoinsUpdated;
            _talentsButton.onClick.AddListener(OnTalentButtonClicked);
            _speedUpgradeButton.onClick.AddListener(OnSpeedUpgradeButtonClicked);
            _dmgUpgradeButton.onClick.AddListener(OnUDamageUpgradeButtonClicked);
            
            if (!IsUnlocked)
                _lockedUi.SetActive(true);
            
            HideTalentWindow();
        }
        
        private void OnDisable()
        {
            EventManager.I.OnBottomMenuHidden -= OnBottomMenuHidden;
            EventManager.I.OnCoinsUpdated -= OnCoinsUpdated;
            _talentsButton.onClick.RemoveListener(OnTalentButtonClicked);
            _speedUpgradeButton.onClick.RemoveListener(OnSpeedUpgradeButtonClicked);
            _dmgUpgradeButton.onClick.RemoveListener(OnUDamageUpgradeButtonClicked);
        }
        
        private void OnBottomMenuHidden(MenuBase menu)
        {
            if (menu is BallsMenu)
                HideTalentWindow();
        }

        private void OnCoinsUpdated(double total, double change)
        {
            UpdateSpeedButtonState();
            UpdateDamageButtonState();
        }

        private void OnTalentButtonClicked()
        {
            StartCoroutine(ToggleTalentWindow());
        }

        private void OnSpeedUpgradeButtonClicked()
        {
            UpgradeSpeed();
        }

        private void OnUDamageUpgradeButtonClicked()
        {
            UpgradeDamage();
        }

        #region Setup

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

        #endregion

        #region Talent Window

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
        private IEnumerator ToggleTalentWindow()
        {
            _isTalentsVisible = !_isTalentsVisible;

            var sequence = DOTween.Sequence();
            
            if (_isTalentsVisible)
            {
                UpdateAllUpgradesUi();
                
                sequence.Append(_rect.DOSizeDelta(new Vector2(_rectStartSize.x, _rectSizeOpenY), 0.25f)
                    .SetEase(Ease.InOutCirc));
                sequence.Append(_upgrades.DOFade(1f, 0.1f));
            }
            else
            {
                sequence.Append(_upgrades.DOFade(0f, 0.1f));
                sequence.Append(_rect.DOSizeDelta(_rectStartSize, 0.25f).SetEase(Ease.InOutCirc));
            }

            while (sequence.IsActive())
            {
                LayoutRebuilder.MarkLayoutForRebuild(_rect);
                yield return null;
            }
        }

        /// <summary>
        /// Closes the talent window.
        /// </summary>
        private void HideTalentWindow()
        {
            _isTalentsVisible = false;
            _rect.sizeDelta = _rectStartSize;
            _upgrades.alpha = 0f;
        }

        #endregion

        #region General Ui

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
        
        /// <summary>
        /// Updates all upgrade UIs
        /// </summary>
        private void UpdateAllUpgradesUi()
        {
            UpdateSpeedButtonState();
            UpdateSpeedUpgradeUi();
            UpdateDamageButtonState();
            UpdateDamageUpgradeUi();
        }

        #endregion
        
        #region Speed
        
        /// <summary>
        /// Upgrades ball speed.
        /// </summary>
        private void UpgradeSpeed()
        {
            if (!GameManager.Data.CanBallSpeedUpgrade(Ball))
                return;
            
            GameManager.Data.UpgradeBallSpeed(Ball);
            UpdateSpeedUpgradeUi();
            UpdateStatsUi();
        }
        
        /// <summary>
        /// Updates the speed button state.
        /// </summary>
        private void UpdateSpeedButtonState()
        {
            if (!_isTalentsVisible)
                return;
            
            _speedUpgradeButton.interactable = GameManager.Data.CanBallSpeedUpgrade(Ball);
        }
        
        /// <summary>
        /// Updates speed upgrade Ui.
        /// </summary>
        private void UpdateSpeedUpgradeUi()
        {
            _speedUpgradeLevelUi.SetText(Data.SpeedLevel.ToString());
            _speedUpgradeCurrentUi.SetText(Helper.GetNumberAsString(GameManager.Data.GetBallSpeed(Ball)));

            var next = Constants.Maxed;
            var cost = Constants.Maxed;

            if (!GameManager.Data.IsBallSpeedMaxLevel(Ball))
            {
                next = Helper.GetNumberAsString(GameManager.Data.GetBallSpeed(Ball, Ball.Data.SpeedLevel + 1));
                cost = "0.00";
            }

            _speedUpgradeNextUi.SetText(next);
            _speedUpgradeCostUi.SetText(cost);
        }

        #endregion
        
        #region Damage
        
        /// <summary>
        /// Upgrades ball damage.
        /// </summary>
        private void UpgradeDamage()
        {
            if (!GameManager.Data.CanBallDamageUpgrade(Ball))
                return;
            
            GameManager.Data.UpgradeBallDamage(Ball);
            UpdateDamageUpgradeUi();
            UpdateStatsUi();
        }
        
        /// <summary>
        /// Updates the Damage button state.
        /// </summary>
        private void UpdateDamageButtonState()
        {
            if (!_isTalentsVisible)
                return;
            
            _dmgUpgradeButton.interactable = GameManager.Data.CanBallDamageUpgrade(Ball);
        }
        
        /// <summary>
        /// Updates damage upgrade Ui.
        /// </summary>
        private void UpdateDamageUpgradeUi()
        {
            _dmgUpgradeLevelUi.SetText(Data.DamageLevel.ToString());
            _dmgUpgradeCurrentUi.SetText(Helper.GetNumberAsString(GameManager.Data.GetBallDamage(Ball)));

            var next = Constants.Maxed;
            var cost = Constants.Maxed;

            if (!GameManager.Data.IsBallDamageMaxLevel(Ball))
            {
                next = Helper.GetNumberAsString(GameManager.Data.GetBallDamage(Ball, Ball.Data.DamageLevel + 1));
                cost = "0.00";
            }

            _dmgUpgradeNextUi.SetText(next);
            _dmgUpgradeCostUi.SetText(cost);
        }

        #endregion
    }
}