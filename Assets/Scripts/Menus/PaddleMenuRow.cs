using System;
using System.Collections;
using System.Collections.Generic;
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
    public class PaddleMenuRow : MonoBehaviour
    {
        [Tab("General")] 
        [SerializeField] private Button _talentsButton;
        [SerializeField] private float _rectSizeOpenY;
        [SerializeField] private CanvasGroup _upgrades;
        
        [Tab("Stats")] 
        [SerializeField] private TMP_Text _speedStatsUi;
        [SerializeField] private TMP_Text _dmgStatsUi;
        [SerializeField] private TMP_Text _dpsStatsUi;
        [SerializeField] private TMP_Text _chcStatsUi;
        [SerializeField] private TMP_Text _chdStatsUi;
        [SerializeField] private TMP_Text _fireRateStatsUi;
        [SerializeField] private TMP_Text _bulletSpeedStatsUi;
        
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

        [Tab("Fire")] 
        [Foldout("Fire Rate")] 
        [SerializeField] private Button _fireRateUpgradeButton;
        [SerializeField] private TMP_Text _fireRateUpgradeLevelUi;
        [SerializeField] private TMP_Text _fireRateUpgradeCurrentUi;
        [SerializeField] private TMP_Text _fireRateUpgradeNextUi;
        [SerializeField] private TMP_Text _fireRateUpgradeCostUi;

        [Foldout("Bullet Speed")] 
        [SerializeField] private Button _bulletSpeedUpgradeButton;
        [SerializeField] private TMP_Text _bulletSpeedUpgradeLevelUi;
        [SerializeField] private TMP_Text _bulletSpeedUpgradeCurrentUi;
        [SerializeField] private TMP_Text _bulletSpeedUpgradeNextUi;
        [SerializeField] private TMP_Text _bulletSpeedUpgradeCostUi;
        
        private bool _isUpgradeWindowVisible;
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
            EventManager.I.OnUpgradePointsUpdated += OnUpgradePointsUpdated;
            EventManager.I.OnDpsUpdated += OnDpsUpdated;
            _talentsButton.onClick.AddListener(OnTalentButtonClicked);
            _speedUpgradeButton.onClick.AddListener(OnSpeedUpgradeButtonClicked);
            _dmgUpgradeButton.onClick.AddListener(OnDamageUpgradeButtonClicked);
            _chcUpgradeButton.onClick.AddListener(OnCriticalChanceUpgradeButtonClicked);
            _chdUpgradeButton.onClick.AddListener(OnCriticalDamageUpgradeButtonClicked);
            _fireRateUpgradeButton.onClick.AddListener(OnCActiveHitsUpgradeButtonClicked);
            _bulletSpeedUpgradeButton.onClick.AddListener(OnCActiveDamageUpgradeButtonClicked);
            
            HideUpgradeWindow();
        }

        private void OnDisable()
        {
            EventManager.I.OnBottomMenuHidden -= OnBottomMenuHidden;
            EventManager.I.OnUpgradePointsUpdated -= OnUpgradePointsUpdated;
            EventManager.I.OnDpsUpdated -= OnDpsUpdated;
            _talentsButton.onClick.RemoveListener(OnTalentButtonClicked);
            _speedUpgradeButton.onClick.RemoveListener(OnSpeedUpgradeButtonClicked);
            _dmgUpgradeButton.onClick.RemoveListener(OnDamageUpgradeButtonClicked);
            _chcUpgradeButton.onClick.RemoveListener(OnCriticalChanceUpgradeButtonClicked);
            _chdUpgradeButton.onClick.RemoveListener(OnCriticalDamageUpgradeButtonClicked);
            _fireRateUpgradeButton.onClick.RemoveListener(OnCActiveHitsUpgradeButtonClicked);
            _bulletSpeedUpgradeButton.onClick.RemoveListener(OnCActiveDamageUpgradeButtonClicked);
        }

        private void Start()
        {
            UpdateStatsUi();
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
            UpdateFireRateButtonState();
            UpdateBulletSpeedButtonState();
        }
        
        private void OnDpsUpdated(Dictionary<int, double> dps)
        {
            if (dps.ContainsKey(0))
                UpdateStatsDpsUi(dps[0]);
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
            UpgradeBulletSpeed();
        }
        
        #region Upgrade Window

        /// <summary>
        ///     Toggles the Upgrade window.
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
        ///     Closes the upgrade window.
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
        ///     Updates the stats UI.
        /// </summary>
        private void UpdateStatsUi()
        {
            _speedStatsUi.SetText(Helper.GetNumberAsString(GameManager.Data.GetPaddleSpeed()));
            _dmgStatsUi.SetText(Helper.GetNumberAsString(GameManager.Data.GetPaddleBulletDamage()));
            _chcStatsUi.SetText(Helper.GetNumberAsString(GameManager.Data.GetPaddleBulletCriticalChance()));
            _chdStatsUi.SetText(Helper.GetNumberAsString(GameManager.Data.GetPaddleBulletCriticalDamage() * 100));
            _fireRateStatsUi.SetText(Helper.GetNumberAsString(GameManager.Data.GetPaddleBulletFireRate()));
            _bulletSpeedStatsUi.SetText(Helper.GetNumberAsString(GameManager.Data.GetPaddleBulletSpeed()));
        }

        /// <summary>
        ///     Updates the stats Dps Ui
        /// </summary>
        /// <returns></returns>
        private void UpdateStatsDpsUi(double dps)
        {
            _dpsStatsUi.SetText(Helper.GetNumberAsString(dps));
        }

        /// <summary>
        ///     Updates all upgrade UIs
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

            UpdateFireRateButtonState();
            UpdateFireRateUpgradeUi();

            UpdateBulletSpeedButtonState();
            UpdateBulletSpeedUpgradeUi();
        }

        #endregion
        
        #region Speed

        /// <summary>
        ///     Upgrades ball speed.
        /// </summary>
        private void UpgradeSpeed()
        {
            if (!GameManager.Data.CanPaddleSpeedUpgrade())
                return;

            GameManager.Data.UpgradePaddleSpeed();
            UpdateSpeedButtonState();
            UpdateSpeedUpgradeUi();
            UpdateStatsUi();
        }

        /// <summary>
        ///     Updates the speed button state.
        /// </summary>
        private void UpdateSpeedButtonState()
        {
            if (!_isUpgradeWindowVisible)
                return;

            _speedUpgradeButton.interactable = GameManager.Data.CanPaddleSpeedUpgrade();
        }

        /// <summary>
        ///     Updates speed upgrade Ui.
        /// </summary>
        private void UpdateSpeedUpgradeUi()
        {
            _speedUpgradeLevelUi.SetText(GameManager.Data.PaddleSpeedLevel.ToString());
            _speedUpgradeCurrentUi.SetText(Helper.GetNumberAsString(GameManager.Data.GetPaddleSpeed()));

            var next = Constants.Maxed;
            var cost = Constants.Maxed;

            if (!GameManager.Data.IsPaddleSpeedMaxLevel())
            {
                next = Helper.GetNumberAsString(GameManager.Data.GetPaddleSpeed(GameManager.Data.PaddleSpeedLevel + 1));
                cost = Helper.GetNumberAsString(GameManager.Data.GetPaddleSpeedUpgradeCost());
            }

            _speedUpgradeNextUi.SetText(next);
            _speedUpgradeCostUi.SetText(cost);
        }

        #endregion
        
        #region Damage

        /// <summary>
        ///     Upgrades damage.
        /// </summary>
        private void UpgradeDamage()
        {
            if (!GameManager.Data.CanPaddleBulletDamageUpgrade())
                return;
            
            GameManager.Data.UpgradePaddleBulletDamage();
            UpdateDamageButtonState();
            UpdateDamageUpgradeUi();
            UpdateStatsUi();
        }

        /// <summary>
        ///     Updates the Damage button state.
        /// </summary>
        private void UpdateDamageButtonState()
        {
            if (!_isUpgradeWindowVisible)
                return;

            _dmgUpgradeButton.interactable = GameManager.Data.CanPaddleBulletDamageUpgrade();
        }

        /// <summary>
        ///     Updates damage upgrade Ui.
        /// </summary>
        private void UpdateDamageUpgradeUi()
        {
            _dmgUpgradeLevelUi.SetText(GameManager.Data.PaddleBulletDamageLevel.ToString());
            _dmgUpgradeCurrentUi.SetText(Helper.GetNumberAsString(GameManager.Data.GetPaddleBulletDamage()));

            var next = Constants.Maxed;
            var cost = Constants.Maxed;

            if (!GameManager.Data.IsPaddleBulletDamageMaxLevel())
            {
                next = Helper.GetNumberAsString(GameManager.Data.GetPaddleBulletDamage(GameManager.Data.PaddleBulletDamageLevel + 1));
                cost = Helper.GetNumberAsString(GameManager.Data.GetPaddleBulletDamageUpgradeCost());
            }

            _dmgUpgradeNextUi.SetText(next);
            _dmgUpgradeCostUi.SetText(cost);
        }
        
        #endregion
        
        #region Critical Chance

        /// <summary>
        ///     Upgrades paddle critical chance.
        /// </summary>
        private void UpgradeCriticalChance()
        {
            if (!GameManager.Data.CanPaddleBulletCriticalChanceUpgrade())
                return;

            GameManager.Data.UpgradePaddleBulletCriticalChance();
            UpdateCriticalChanceButtonState();
            UpdateCriticalChanceUpgradeUi();
            UpdateStatsUi();
        }

        /// <summary>
        ///     Updates the critical chance button state.
        /// </summary>
        private void UpdateCriticalChanceButtonState()
        {
            if (!_isUpgradeWindowVisible)
                return;

            _chcUpgradeButton.interactable = GameManager.Data.CanPaddleBulletCriticalChanceUpgrade();
        }

        /// <summary>
        ///     Updates critical chance upgrade Ui.
        /// </summary>
        private void UpdateCriticalChanceUpgradeUi()
        {
            _chcUpgradeLevelUi.SetText(GameManager.Data.PaddleBulletCriticalChanceLevel.ToString());
            _chcUpgradeCurrentUi.SetText(Helper.GetNumberAsString(GameManager.Data.GetPaddleBulletCriticalChance()));
            
            var next = Constants.Maxed;
            var cost = Constants.Maxed;
            
            if (!GameManager.Data.IsPaddleBulletCriticalChanceMaxLevel())
            {
                next = Helper.GetNumberAsString(GameManager.Data.GetPaddleBulletCriticalChance(GameManager.Data.PaddleBulletCriticalChanceLevel + 1));
                cost = Helper.GetNumberAsString(GameManager.Data.GetPaddleBulletCriticalChanceUpgradeCost());
            }

            _chcUpgradeNextUi.SetText(next);
            _chcUpgradeCostUi.SetText(cost);
        }

        #endregion

        #region Critical Damage

        /// <summary>
        ///     Upgrades paddle critical damage.
        /// </summary>
        private void UpgradeCriticalDamage()
        {
            if (!GameManager.Data.CanPaddleBulletCriticalDamageUpgrade())
                return;

            GameManager.Data.UpgradePaddleBulletCriticalDamage();
            UpdateCriticalDamageButtonState();
            UpdateCriticalDamageUpgradeUi();
            UpdateStatsUi();
        }

        /// <summary>
        ///     Updates the critical damage button state.
        /// </summary>
        private void UpdateCriticalDamageButtonState()
        {
            if (!_isUpgradeWindowVisible)
                return;

            _chdUpgradeButton.interactable = GameManager.Data.CanPaddleBulletCriticalDamageUpgrade();
        }

        /// <summary>
        ///     Updates critical damage upgrade Ui.
        /// </summary>
        private void UpdateCriticalDamageUpgradeUi()
        {
            _chdUpgradeLevelUi.SetText(GameManager.Data.PaddleBulletCriticalDamageLevel.ToString());
            _chdUpgradeCurrentUi.SetText(Helper.GetNumberAsString(GameManager.Data.GetPaddleBulletCriticalDamage() * 100));

            var next = Constants.Maxed;
            var cost = Constants.Maxed;

            if (!GameManager.Data.IsPaddleBulletCriticalDamageMaxLevel())
            {
                next = Helper.GetNumberAsString(GameManager.Data.GetPaddleBulletCriticalDamage(GameManager.Data.PaddleBulletCriticalDamageLevel + 1) * 100);
                cost = Helper.GetNumberAsString(GameManager.Data.GetPaddleBulletCriticalDamageUpgradeCost());
            }

            _chdUpgradeNextUi.SetText(next);
            _chdUpgradeCostUi.SetText(cost);
        }

        #endregion
        
        #region Fire Rate

        /// <summary>
        ///     Upgrades fire rate.
        /// </summary>
        private void UpgradeActiveHits()
        {
            if (!GameManager.Data.CanPaddleBulletFireRateUpgrade())
                return;

            GameManager.Data.UpgradePaddleBulletFireRate();
            UpdateFireRateButtonState();
            UpdateFireRateUpgradeUi();
            UpdateStatsUi();
        }

        /// <summary>
        ///     Updates the fire rate button state.
        /// </summary>
        private void UpdateFireRateButtonState()
        {
            if (!_isUpgradeWindowVisible)
                return;

            _fireRateUpgradeButton.interactable = GameManager.Data.CanPaddleBulletFireRateUpgrade();
        }

        /// <summary>
        ///     Updates the fire rate upgrade Ui.
        /// </summary>
        private void UpdateFireRateUpgradeUi()
        {
            _fireRateUpgradeLevelUi.SetText(GameManager.Data.PaddleBulletFireRateLevel.ToString());
            _fireRateUpgradeCurrentUi.SetText(Helper.GetNumberAsString(GameManager.Data.GetPaddleBulletFireRate()));

            var next = Constants.Maxed;
            var cost = Constants.Maxed;

            if (!GameManager.Data.IsPaddleBulletFireRateMaxLevel())
            {
                next = GameManager.Data.GetPaddleBulletFireRate(GameManager.Data.PaddleBulletFireRateLevel + 1).ToString();
                cost = Helper.GetNumberAsString(GameManager.Data.GetPaddleBulletFireRateUpgradeCost());
            }

            _fireRateUpgradeNextUi.SetText(next);
            _fireRateUpgradeCostUi.SetText(cost);
        }

        #endregion

        #region Bullet speed

        /// <summary>
        ///     Upgrades bullet speed.
        /// </summary>
        private void UpgradeBulletSpeed()
        {
            if (!GameManager.Data.CanPaddleBulletSpeedUpgrade())
                return;

            GameManager.Data.UpgradePaddleBulletSpeed();
            UpdateBulletSpeedButtonState();
            UpdateBulletSpeedUpgradeUi();
            UpdateStatsUi();
        }

        /// <summary>
        ///     Updates the bullet speed button state.
        /// </summary>
        private void UpdateBulletSpeedButtonState()
        {
            if (!_isUpgradeWindowVisible)
                return;

            _bulletSpeedUpgradeButton.interactable = GameManager.Data.CanPaddleBulletSpeedUpgrade();
        }

        /// <summary>
        ///     Updates bullet speed upgrade Ui.
        /// </summary>
        private void UpdateBulletSpeedUpgradeUi()
        {
            _bulletSpeedUpgradeLevelUi.SetText(GameManager.Data.PaddleBulletSpeedLevel.ToString());
            _bulletSpeedUpgradeCurrentUi.SetText(Helper.GetNumberAsString(GameManager.Data.GetPaddleBulletSpeed() * 100));

            var next = Constants.Maxed;
            var cost = Constants.Maxed;

            if (!GameManager.Data.IsPaddleBulletSpeedMaxLevel())
            {
                next = Helper.GetNumberAsString(GameManager.Data.GetPaddleBulletSpeed(GameManager.Data.PaddleBulletSpeedLevel + 1) * 100);
                cost = Helper.GetNumberAsString(GameManager.Data.GetPaddleBulletSpeedUpgradeCost());
            }

            _bulletSpeedUpgradeNextUi.SetText(next);
            _bulletSpeedUpgradeCostUi.SetText(cost);
        }

        #endregion
    }
}