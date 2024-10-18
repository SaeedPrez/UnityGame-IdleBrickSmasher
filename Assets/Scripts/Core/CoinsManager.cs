using Enums;
using TMPro;
using UnityEngine;
using Utilities;

namespace Core
{
    public class CoinsManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text _coinsValueUi;

        private void OnEnable()
        {
            EventManager.I.OnGameStateChanged += OnGameStateChanged;
            EventManager.I.OnBrickDamaged += OnBrickDamaged;
            EventManager.I.OnLeveledUp += OnLeveledUp;
        }
        
        private void OnDisable()
        {
            EventManager.I.OnGameStateChanged -= OnGameStateChanged;
            EventManager.I.OnBrickDamaged -= OnBrickDamaged;
            EventManager.I.OnLeveledUp -= OnLeveledUp;
        }
        
        private void OnGameStateChanged(EGameState state)
        {
            if (state is EGameState.Loaded)
                UpdateCoinsValueUi();
        }

        private void OnBrickDamaged(Brick brick, Ball ball, double damage, bool destroyed)
        {
            if (destroyed)
                AddCoins(GameManager.Data.GetCoinsForBrickDestroyed(brick));
        }

        private void OnLeveledUp(int level)
        {
            AddCoins(GameManager.Data.GetCoinsForLeveledUp());
        }
        
        /// <summary>
        /// Adds coins.
        /// </summary>
        /// <param name="amount"></param>
        private void AddCoins(double amount)
        {
            GameManager.Data.CoinsCurrent += amount;
            UpdateCoinsValueUi();
            
            EventManager.I.TriggerCoinsUpdated(GameManager.Data.CoinsCurrent, amount);
        }

        /// <summary>
        /// Subtract coins.
        /// </summary>
        /// <param name="amount"></param>
        private void SubtractCoins(double amount)
        {
            GameManager.Data.CoinsCurrent -= amount;
            UpdateCoinsValueUi();
            
            EventManager.I.TriggerCoinsUpdated(GameManager.Data.CoinsCurrent, -amount);
        }

        /// <summary>
        /// Returns if there is enough coins.
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool CanAfford(double amount)
        {
            return GameManager.Data.CoinsCurrent >= amount;
        }
        
        /// <summary>
        /// Updates coins value Ui.
        /// </summary>
        private void UpdateCoinsValueUi()
        {
            _coinsValueUi.SetText(Helper.GetNumberAsString(GameManager.Data.CoinsCurrent));
        }
    }
}