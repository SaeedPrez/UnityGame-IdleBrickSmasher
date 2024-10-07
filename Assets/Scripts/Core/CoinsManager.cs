using Prez.Data;
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
        }
        
        private void OnDisable()
        {
            _event.OnBrickDestroyed -= OnBrickDestroyed;
        }
        
        private void OnBrickDestroyed(Brick brick, long maxHealth)
        {
            AddCoins(_gameData.GetCoins(maxHealth));
        }

        /// <summary>
        /// Adds coins.
        /// </summary>
        /// <param name="amount"></param>
        private void AddCoins(long amount)
        {
            _gameData.Coins.Add(amount);
            UpdateCoinsValueUi();
            
            _event.TriggerCoinsGained(amount);
        }

        /// <summary>
        /// Subtract coins.
        /// </summary>
        /// <param name="amount"></param>
        private void SubtractCoins(long amount)
        {
            _gameData.Coins.Subtract(amount);
            UpdateCoinsValueUi();
            
            _event.TriggerCoinsUsed(amount);
        }

        /// <summary>
        /// Returns if there is enough coins.
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool CanAfford(long amount)
        {
            return _gameData.Coins.AsLong >= amount;
        }
        
        /// <summary>
        /// Updates coins value Ui.
        /// </summary>
        private void UpdateCoinsValueUi()
        {
            _coinsValueUi.text = _gameData.Coins.AsString;
        }
    }
}