using Prez.Data;
using Prez.Utilities;
using TMPro;
using UnityEngine;

namespace Prez.Core
{
    public class CoinsManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text _coinsValueUi;
        
        private EventManager _event;
        private GameData _gameData;

        private void Awake()
        {
            _event = EventManager.I;
            _gameData = GameManager.I.Data;
        }

        private void OnEnable()
        {
            _event.OnBrickDestroyed += OnBrickDestroyed;
            _event.OnLeveledUp += OnLeveledUp;
        }
        
        private void OnDisable()
        {
            _event.OnBrickDestroyed -= OnBrickDestroyed;
            _event.OnLeveledUp -= OnLeveledUp;
        }
        
        private void OnBrickDestroyed(Brick brick, double maxHealth)
        {
            AddCoins(_gameData.GetCoinsGainedPerHealth(maxHealth));
        }

        private void OnLeveledUp(int level)
        {
            AddCoins(_gameData.GetCoinsGainedPerLevel(level - 1));
        }
        
        /// <summary>
        /// Adds coins.
        /// </summary>
        /// <param name="amount"></param>
        private void AddCoins(double amount)
        {
            _gameData.Coins += amount;
            UpdateCoinsValueUi();
            
            _event.TriggerCoinsGained(amount);
        }

        /// <summary>
        /// Subtract coins.
        /// </summary>
        /// <param name="amount"></param>
        private void SubtractCoins(double amount)
        {
            _gameData.Coins -= amount;
            UpdateCoinsValueUi();
            
            _event.TriggerCoinsUsed(amount);
        }

        /// <summary>
        /// Returns if there is enough coins.
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool CanAfford(double amount)
        {
            return _gameData.Coins >= amount;
        }
        
        /// <summary>
        /// Updates coins value Ui.
        /// </summary>
        private void UpdateCoinsValueUi()
        {
            _coinsValueUi.SetText(Helper.GetNumberAsString(_gameData.Coins));
        }
    }
}