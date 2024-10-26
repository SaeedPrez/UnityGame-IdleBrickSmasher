using System.Collections;
using DG.Tweening;
using Prez.Core;
using Prez.Data;
using Prez.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VInspector;

namespace Prez.Menus
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
        [SerializeField] private TMP_Text _chcStatsUi;
        [SerializeField] private TMP_Text _chdStatsUi;
        [SerializeField] private TMP_Text _activeHitsStatsUi;
        [SerializeField] private TMP_Text _activeDmgStatsUi;

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

        [Tab("Critical")] 
        [Foldout("Critical Hits")]
        [SerializeField] private Button _chcUpgradeButton;
        [SerializeField] private TMP_Text _chcUpgradeLevelUi;
        [SerializeField] private TMP_Text _chcUpgradeCurrentUi;
        [SerializeField] private TMP_Text _chcUpgradeNextUi;
        [SerializeField] private TMP_Text _chcUpgradeCostUi;
        [Foldout("Critical Damage")]
        [SerializeField] private Button _chdUpgradeButton;
        [SerializeField] private TMP_Text _chdUpgradeLevelUi;
        [SerializeField] private TMP_Text _chdUpgradeCurrentUi;
        [SerializeField] private TMP_Text _chdUpgradeNextUi;
        [SerializeField] private TMP_Text _chdUpgradeCostUi;

        [Tab("Active")]
        [Foldout("Active Hits")]
        [SerializeField] private Button _activeHitsUpgradeButton;
        [SerializeField] private TMP_Text _activeHitsUpgradeLevelUi;
        [SerializeField] private TMP_Text _activeHitsUpgradeCurrentUi;
        [SerializeField] private TMP_Text _activeHitsUpgradeNextUi;
        [SerializeField] private TMP_Text _activeHitsUpgradeCostUi;
        [Foldout("Active Damage")]
        [SerializeField] private Button _activeDmgUpgradeButton;
        [SerializeField] private TMP_Text _activeDmgUpgradeLevelUi;
        [SerializeField] private TMP_Text _activeDmgUpgradeCurrentUi;
        [SerializeField] private TMP_Text _activeDmgUpgradeNextUi;
        [SerializeField] private TMP_Text _activeDmgUpgradeCostUi;
        
        public Ball Ball { get; private set; }
        public BallData Data { get; private set; }
        public bool IsUnlocked { get; private set; }

        private bool _isUpgradeWindowVisible;
        private RectTransform _rect;
        private Vector2 _rectStartSize;
        private Coroutine _dpsUiCoroutine;

        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
            _rectStartSize = _rect.sizeDelta;
        }

        private void OnEnable()
        {
            EventManager.I.OnBottomMenuHidden += OnBottomMenuHidden;
            EventManager.I.OnUpgradePointsUpdated += OnUpgradePointsUpdated;
            _talentsButton.onClick.AddListener(OnTalentButtonClicked);
            _speedUpgradeButton.onClick.AddListener(OnSpeedUpgradeButtonClicked);
            _dmgUpgradeButton.onClick.AddListener(OnDamageUpgradeButtonClicked);
            _chcUpgradeButton.onClick.AddListener(OnCriticalChanceUpgradeButtonClicked);
            _chdUpgradeButton.onClick.AddListener(OnCriticalDamageUpgradeButtonClicked);
            _activeHitsUpgradeButton.onClick.AddListener(OnCActiveHitsUpgradeButtonClicked);
            _activeDmgUpgradeButton.onClick.AddListener(OnCActiveDamageUpgradeButtonClicked);
            
            if (!IsUnlocked)
                _lockedUi.SetActive(true);
            
            HideUpgradeWindow();
            StartCoroutine(UpdateStatsDpsUi());
        }
        
        private void OnDisable()
        {
            EventManager.I.OnBottomMenuHidden -= OnBottomMenuHidden;
            EventManager.I.OnUpgradePointsUpdated -= OnUpgradePointsUpdated;
            _talentsButton.onClick.RemoveListener(OnTalentButtonClicked);
            _speedUpgradeButton.onClick.RemoveListener(OnSpeedUpgradeButtonClicked);
            _dmgUpgradeButton.onClick.RemoveListener(OnDamageUpgradeButtonClicked);
            _chcUpgradeButton.onClick.RemoveListener(OnCriticalChanceUpgradeButtonClicked);
            _chdUpgradeButton.onClick.RemoveListener(OnCriticalDamageUpgradeButtonClicked);
            _activeHitsUpgradeButton.onClick.RemoveListener(OnCActiveHitsUpgradeButtonClicked);
            _activeDmgUpgradeButton.onClick.RemoveListener(OnCActiveDamageUpgradeButtonClicked);
            StopCoroutine(UpdateStatsDpsUi());
        }
        
        private void OnBottomMenuHidden(MenuBase menu)
        {
            if (menu is BallsMenu)
                HideUpgradeWindow();
        }

        private void OnUpgradePointsUpdated(double total, double change)
        {
            UpdateSpeedButtonState();
            UpdateDamageButtonState();
            UpdateCriticalChanceButtonState();
            UpdateCriticalDamageButtonState();
            UpdateActiveHitsButtonState();
            UpdateActiveDamageButtonState();
        }

        private void OnTalentButtonClicked()
        {
            StartCoroutine(ToggleUpgradeWindow());
        }

        private void OnSpeedUpgradeButtonClicked()
        {
            UpgradeSpeed();
        }

        private void OnDamageUpgradeButtonClicked()
        {
            UpgradeDamage();
        }

        private void OnCriticalChanceUpgradeButtonClicked()
        {
            UpgradeCriticalChance();
        }
        
        private void OnCriticalDamageUpgradeButtonClicked()
        {
            UpgradeCriticalDamage();
        }

        private void OnCActiveHitsUpgradeButtonClicked()
        {
            UpgradeActiveHits();
        }

        private void OnCActiveDamageUpgradeButtonClicked()
        {
            UpgradeActiveDamage();
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

        #region Upgrade Window

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
        /// Toggles the Upgrade window.
        /// </summary>
        private IEnumerator ToggleUpgradeWindow()
        {
            _isUpgradeWindowVisible = !_isUpgradeWindowVisible;

            var sequence = DOTween.Sequence();
            
            if (_isUpgradeWindowVisible)
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
        /// Closes the upgrade window.
        /// </summary>
        private void HideUpgradeWindow()
        {
            _isUpgradeWindowVisible = false;
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
            _chcStatsUi.SetText(Helper.GetNumberAsString(GameManager.Data.GetBallCriticalChance(Ball)));
            _chdStatsUi.SetText(Helper.GetNumberAsString(GameManager.Data.GetBallCriticalDamage(Ball) * 100));
            _activeHitsStatsUi.SetText(GameManager.Data.GetBallActiveHits(Ball).ToString());
            _activeDmgStatsUi.SetText(Helper.GetNumberAsString(GameManager.Data.GetBallActiveDamage(Ball) * 100));
            _dpsStatsUi.SetText(Helper.GetNumberAsString(Ball.Dps));
        }

        /// <summary>
        /// Updates he stats Dps Ui
        /// </summary>
        /// <returns></returns>
        private IEnumerator UpdateStatsDpsUi()
        {
            while (true)
            {
                _dpsStatsUi.SetText(Helper.GetNumberAsString(Ball.Dps));
                yield return new WaitForSeconds(3.33f);
            }
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
            
            UpdateCriticalChanceButtonState();
            UpdateCriticalChanceUpgradeUi();
            
            UpdateCriticalDamageButtonState();
            UpdateCriticalDamageUpgradeUi();
            
            UpdateActiveHitsButtonState();
            UpdateActiveHitsUpgradeUi();
            
            UpdateActiveDamageButtonState();
            UpdateActiveDamageUpgradeUi();
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
            UpdateSpeedButtonState();
            UpdateSpeedUpgradeUi();
            UpdateStatsUi();
        }
        
        /// <summary>
        /// Updates the speed button state.
        /// </summary>
        private void UpdateSpeedButtonState()
        {
            if (!_isUpgradeWindowVisible)
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
                cost = Helper.GetNumberAsString(GameManager.Data.GetBallSpeedUpgradeCost(Ball));
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
            UpdateDamageButtonState();
            UpdateDamageUpgradeUi();
            UpdateStatsUi();
        }
        
        /// <summary>
        /// Updates the Damage button state.
        /// </summary>
        private void UpdateDamageButtonState()
        {
            if (!_isUpgradeWindowVisible)
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
                cost = Helper.GetNumberAsString(GameManager.Data.GetBallDamageUpgradeCost(Ball));
            }

            _dmgUpgradeNextUi.SetText(next);
            _dmgUpgradeCostUi.SetText(cost);
        }

        #endregion
        
        #region Critical Chance
        
        /// <summary>
        /// Upgrades ball critical chance.
        /// </summary>
        private void UpgradeCriticalChance()
        {
            if (!GameManager.Data.CanBallCriticalChanceUpgrade(Ball))
                return;
            
            GameManager.Data.UpgradeBallCriticalChance(Ball);
            UpdateCriticalChanceButtonState();
            UpdateCriticalChanceUpgradeUi();
            UpdateStatsUi();
        }
        
        /// <summary>
        /// Updates the critical chance button state.
        /// </summary>
        private void UpdateCriticalChanceButtonState()
        {
            if (!_isUpgradeWindowVisible)
                return;
            
            _chcUpgradeButton.interactable = GameManager.Data.CanBallCriticalChanceUpgrade(Ball);
        }
        
        /// <summary>
        /// Updates critical chance upgrade Ui.
        /// </summary>
        private void UpdateCriticalChanceUpgradeUi()
        {
            _chcUpgradeLevelUi.SetText(Data.CriticalChanceLevel.ToString());
            _chcUpgradeCurrentUi.SetText(Helper.GetNumberAsString(GameManager.Data.GetBallCriticalChance(Ball)));

            var next = Constants.Maxed;
            var cost = Constants.Maxed;

            if (!GameManager.Data.IsBallCriticalChanceMaxLevel(Ball))
            {
                next = Helper.GetNumberAsString(GameManager.Data.GetBallCriticalChance(Ball, Ball.Data.CriticalChanceLevel + 1));
                cost = Helper.GetNumberAsString(GameManager.Data.GetBallCriticalChanceUpgradeCost(Ball));
            }

            _chcUpgradeNextUi.SetText(next);
            _chcUpgradeCostUi.SetText(cost);
        }

        #endregion
        
        #region Critical Damage
        
        /// <summary>
        /// Upgrades ball critical damage.
        /// </summary>
        private void UpgradeCriticalDamage()
        {
            if (!GameManager.Data.CanBallCriticalDamageUpgrade(Ball))
                return;
            
            GameManager.Data.UpgradeBallCriticalDamage(Ball);
            UpdateCriticalDamageButtonState();
            UpdateCriticalDamageUpgradeUi();
            UpdateStatsUi();
        }
        
        /// <summary>
        /// Updates the critical damage button state.
        /// </summary>
        private void UpdateCriticalDamageButtonState()
        {
            if (!_isUpgradeWindowVisible)
                return;
            
            _chdUpgradeButton.interactable = GameManager.Data.CanBallCriticalDamageUpgrade(Ball);
        }
        
        /// <summary>
        /// Updates critical damage upgrade Ui.
        /// </summary>
        private void UpdateCriticalDamageUpgradeUi()
        {
            _chdUpgradeLevelUi.SetText(Data.CriticalDamageLevel.ToString());
            _chdUpgradeCurrentUi.SetText(Helper.GetNumberAsString(GameManager.Data.GetBallCriticalDamage(Ball) * 100));

            var next = Constants.Maxed;
            var cost = Constants.Maxed;

            if (!GameManager.Data.IsBallCriticalDamageMaxLevel(Ball))
            {
                next = Helper.GetNumberAsString(GameManager.Data.GetBallCriticalDamage(Ball, Ball.Data.CriticalDamageLevel + 1) * 100);
                cost = Helper.GetNumberAsString(GameManager.Data.GetBallCriticalDamageUpgradeCost(Ball));
            }

            _chdUpgradeNextUi.SetText(next);
            _chdUpgradeCostUi.SetText(cost);
        }

        #endregion
        
        #region Active Hits
        
        /// <summary>
        /// Upgrades ball active hits.
        /// </summary>
        private void UpgradeActiveHits()
        {
            if (!GameManager.Data.CanBallActiveHitsUpgrade(Ball))
                return;
            
            GameManager.Data.UpgradeBallActiveHits(Ball);
            UpdateActiveHitsButtonState();
            UpdateActiveHitsUpgradeUi();
            UpdateStatsUi();
        }
        
        /// <summary>
        /// Updates the active hits button state.
        /// </summary>
        private void UpdateActiveHitsButtonState()
        {
            if (!_isUpgradeWindowVisible)
                return;
            
            _activeHitsUpgradeButton.interactable = GameManager.Data.CanBallActiveHitsUpgrade(Ball);
        }
        
        /// <summary>
        /// Updates active hits upgrade Ui.
        /// </summary>
        private void UpdateActiveHitsUpgradeUi()
        {
            _activeHitsUpgradeLevelUi.SetText(Data.ActiveHitsLevel.ToString());
            _activeHitsUpgradeCurrentUi.SetText(GameManager.Data.GetBallActiveHits(Ball).ToString());

            var next = Constants.Maxed;
            var cost = Constants.Maxed;

            if (!GameManager.Data.IsBallActiveHitsMaxLevel(Ball))
            {
                next = GameManager.Data.GetBallActiveHits(Ball, Ball.Data.ActiveHitsLevel + 1).ToString();
                cost = Helper.GetNumberAsString(GameManager.Data.GetBallActiveHitsUpgradeCost(Ball));
            }

            _activeHitsUpgradeNextUi.SetText(next);
            _activeHitsUpgradeCostUi.SetText(cost);
        }

        #endregion
        
        #region Active Damage
        
        /// <summary>
        /// Upgrades ball active damage.
        /// </summary>
        private void UpgradeActiveDamage()
        {
            if (!GameManager.Data.CanBallActiveDamageUpgrade(Ball))
                return;
            
            GameManager.Data.UpgradeBallActiveDamage(Ball);
            UpdateActiveDamageButtonState();
            UpdateActiveDamageUpgradeUi();
            UpdateStatsUi();
        }
        
        /// <summary>
        /// Updates the active damage button state.
        /// </summary>
        private void UpdateActiveDamageButtonState()
        {
            if (!_isUpgradeWindowVisible)
                return;
            
            _activeDmgUpgradeButton.interactable = GameManager.Data.CanBallActiveDamageUpgrade(Ball);
        }
        
        /// <summary>
        /// Updates active damage upgrade Ui.
        /// </summary>
        private void UpdateActiveDamageUpgradeUi()
        {
            _activeDmgUpgradeLevelUi.SetText(Data.ActiveDamageLevel.ToString());
            _activeDmgUpgradeCurrentUi.SetText(Helper.GetNumberAsString(GameManager.Data.GetBallActiveDamage(Ball) * 100));

            var next = Constants.Maxed;
            var cost = Constants.Maxed;

            if (!GameManager.Data.IsBallActiveDamageMaxLevel(Ball))
            {
                next = Helper.GetNumberAsString(GameManager.Data.GetBallActiveDamage(Ball, Ball.Data.ActiveDamageLevel + 1) * 100);
                cost = Helper.GetNumberAsString(GameManager.Data.GetBallActiveDamageUpgradeCost(Ball));
            }

            _activeDmgUpgradeNextUi.SetText(next);
            _activeDmgUpgradeCostUi.SetText(cost);
        }

        #endregion
    }
}