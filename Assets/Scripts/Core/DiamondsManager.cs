using Enums;
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
                UpdateDiamondsValueUi();
        }
        
        private void OnBrickDamaged(Brick brick, Ball ball, double damage, bool destroyed)
        {
            // TODO: Chance to get diamonds?
        }

        private void OnLeveledUp(int level)
        {
            AddDiamonds(GameManager.Data.GetDiamondsForLeveledUp());
        }
        
        /// <summary>
        /// Adds diamonds.
        /// </summary>
        /// <param name="amount"></param>
        private void AddDiamonds(double amount)
        {
            GameManager.Data.DiamondsCurrent += amount;
            UpdateDiamondsValueUi();
            
            EventManager.I.TriggerDiamondsGained(amount);
        }
        
        /// <summary>
        /// Subtract diamonds.
        /// </summary>
        /// <param name="amount"></param>
        private void SubtractDiamonds(double amount)
        {
            GameManager.Data.DiamondsCurrent -= amount;
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
            return GameManager.Data.DiamondsCurrent >= amount;
        }
        
        /// <summary>
        /// Updates diamonds value Ui.
        /// </summary>
        private void UpdateDiamondsValueUi()
        {
            _diamondsValueUi.SetText(Helper.GetNumberAsString(GameManager.Data.DiamondsCurrent));
        }
    }
}