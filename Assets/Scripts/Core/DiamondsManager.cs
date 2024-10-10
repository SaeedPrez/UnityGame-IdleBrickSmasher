using Prez.Data;
using Prez.Utilities;
using TMPro;
using UnityEngine;

namespace Prez.Core
{
    public class DiamondsManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text _diamondsValueUi;
        
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
            // TODO: Chance to get diamonds?
        }

        private void OnLeveledUp(int level)
        {
            AddDiamonds(_gameData.GetDiamondsGainedPerLevel(level - 1));
        }
        
        /// <summary>
        /// Adds diamonds.
        /// </summary>
        /// <param name="amount"></param>
        private void AddDiamonds(double amount)
        {
            _gameData.Diamonds += amount;
            UpdateDiamondsValueUi();
            
            _event.TriggerDiamondsGained(amount);
        }
        
        /// <summary>
        /// Subtract diamonds.
        /// </summary>
        /// <param name="amount"></param>
        private void SubtractDiamonds(double amount)
        {
            _gameData.Diamonds -= amount;
            UpdateDiamondsValueUi();
            
            _event.TriggerDiamondsUsed(amount);
        }
        
        /// <summary>
        /// Returns if there is enough diamonds.
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool CanAfford(double amount)
        {
            return _gameData.Diamonds >= amount;
        }
        
        /// <summary>
        /// Updates diamonds value Ui.
        /// </summary>
        private void UpdateDiamondsValueUi()
        {
            _diamondsValueUi.SetText(Helper.GetNumberAsString(_gameData.Diamonds));
        }
    }
}