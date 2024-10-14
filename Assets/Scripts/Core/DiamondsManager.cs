using TMPro;
using UnityEngine;
using Utilities;

namespace Core
{
    public class DiamondsManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text _diamondsValueUi;

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
            // TODO: Chance to get diamonds?
        }

        private void OnLeveledUp(int level)
        {
            AddDiamonds(GameManager.Data.GetDiamondsGainedPerLevel(level - 1));
        }
        
        /// <summary>
        /// Adds diamonds.
        /// </summary>
        /// <param name="amount"></param>
        private void AddDiamonds(double amount)
        {
            GameManager.Data.Diamonds += amount;
            UpdateDiamondsValueUi();
            
            EventManager.I.TriggerDiamondsGained(amount);
        }
        
        /// <summary>
        /// Subtract diamonds.
        /// </summary>
        /// <param name="amount"></param>
        private void SubtractDiamonds(double amount)
        {
            GameManager.Data.Diamonds -= amount;
            UpdateDiamondsValueUi();
            
            EventManager.I.TriggerDiamondsUsed(amount);
        }
        
        /// <summary>
        /// Returns if there is enough diamonds.
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool CanAfford(double amount)
        {
            return GameManager.Data.Diamonds >= amount;
        }
        
        /// <summary>
        /// Updates diamonds value Ui.
        /// </summary>
        private void UpdateDiamondsValueUi()
        {
            _diamondsValueUi.SetText(Helper.GetNumberAsString(GameManager.Data.Diamonds));
        }
    }
}