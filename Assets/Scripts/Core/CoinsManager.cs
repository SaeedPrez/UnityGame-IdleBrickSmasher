using Data;
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
            EventManager.I.OnBrickDestroyed += OnBrickDestroyed;
            EventManager.I.OnLeveledUp += OnLeveledUp;
        }
        
        private void OnDisable()
        {
            EventManager.I.OnBrickDestroyed -= OnBrickDestroyed;
            EventManager.I.OnLeveledUp -= OnLeveledUp;
        }
        
        private void OnBrickDestroyed(Brick brick, double maxHealth)
        {
            AddCoins(GameManager.Data.GetCoinsGainedPerHealth(maxHealth));
        }

        private void OnLeveledUp(int level)
        {
            AddCoins(GameManager.Data.GetCoinsGainedPerLevel(level - 1));
        }
        
        /// <summary>
        /// Adds coins.
        /// </summary>
        /// <param name="amount"></param>
        private void AddCoins(double amount)
        {
            GameManager.Data.Coins += amount;
            UpdateCoinsValueUi();
            
            EventManager.I.TriggerCoinsGained(amount);
        }

        /// <summary>
        /// Subtract coins.
        /// </summary>
        /// <param name="amount"></param>
        private void SubtractCoins(double amount)
        {
            GameManager.Data.Coins -= amount;
            UpdateCoinsValueUi();
            
            EventManager.I.TriggerCoinsUsed(amount);
        }

        /// <summary>
        /// Returns if there is enough coins.
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool CanAfford(double amount)
        {
            return GameManager.Data.Coins >= amount;
        }
        
        /// <summary>
        /// Updates coins value Ui.
        /// </summary>
        private void UpdateCoinsValueUi()
        {
            _coinsValueUi.SetText(Helper.GetNumberAsString(GameManager.Data.Coins));
        }
    }
}