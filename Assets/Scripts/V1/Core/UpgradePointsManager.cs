using Prez.V1.Data;
using Prez.V1.Enums;
using Prez.V1.Utilities;
using TMPro;
using UnityEngine;

namespace Prez.V1.Core
{
    public class UpgradePointsManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text _upgradePointsValueUi;

        private void OnEnable()
        {
            EventManager.I.OnGameStateChanged += OnGameStateChanged;
            EventManager.I.OnLeveledUp += OnLeveledUp;
            EventManager.I.OnBallUpgraded += OnBallUpgraded;
            EventManager.I.OnPaddleUpgraded += OnPaddleUpgraded;
            EventManager.I.OnPaddleBulletUpgraded += OnPaddleUpgraded;
            EventManager.I.OnBrickDamaged += OnBrickDamaged;
        }

        private void OnDisable()
        {
            EventManager.I.OnGameStateChanged -= OnGameStateChanged;
            EventManager.I.OnLeveledUp -= OnLeveledUp;
            EventManager.I.OnBallUpgraded -= OnBallUpgraded;
            EventManager.I.OnPaddleUpgraded -= OnPaddleUpgraded;
            EventManager.I.OnPaddleBulletUpgraded -= OnPaddleUpgraded;
            EventManager.I.OnBrickDamaged -= OnBrickDamaged;
        }

        private void OnGameStateChanged(EGameState state)
        {
            if (state is EGameState.Loaded)
                UpdateUpgradePointsValueUi();
        }

        private void OnLeveledUp(int level)
        {
            AddUpgradePoints(GameManager.Data.GetUpgradePointsForLevel());
        }

        private void OnBallUpgraded(Ball ball, EStat stat, float cost)
        {
            SubtractUpgradePoints(cost);
        }
        
        private void OnPaddleUpgraded(EStat stat, float cost)
        {
            SubtractUpgradePoints(cost);
        }

        private void OnBrickDamaged(DamageData data)
        {
            if (!data.BrickDestroyed)
                return;

            if (!data.Brick.IsSpecial)
                return;
            
            AddUpgradePoints(1f);
        }

        /// <summary>
        ///     Adds upgrade points.
        /// </summary>
        /// <param name="amount"></param>
        private void AddUpgradePoints(float amount)
        {
            if (amount == 0)
                return;

            GameManager.Data.UpgradePointsCurrent += amount;
            UpdateUpgradePointsValueUi();

            EventManager.I.TriggerUpgradePointsUpdated(GameManager.Data.UpgradePointsCurrent, amount);
        }

        /// <summary>
        ///     Subtract upgrade points.
        /// </summary>
        /// <param name="amount"></param>
        private void SubtractUpgradePoints(float amount)
        {
            if (amount == 0)
                return;

            GameManager.Data.UpgradePointsCurrent -= amount;
            UpdateUpgradePointsValueUi();

            EventManager.I.TriggerUpgradePointsUpdated(GameManager.Data.UpgradePointsCurrent, -amount);
        }

        /// <summary>
        ///     Returns if there is enough upgrade points.
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool CanAfford(double amount)
        {
            return GameManager.Data.UpgradePointsCurrent >= amount;
        }

        /// <summary>
        ///     Updates upgrade points value Ui.
        /// </summary>
        private void UpdateUpgradePointsValueUi()
        {
            _upgradePointsValueUi.SetText(Helper.GetNumberAsString(GameManager.Data.UpgradePointsCurrent));
        }
    }
}