using Prez.Enums;
using Prez.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Prez.Core
{
    public class TalentPointManager : MonoBehaviour
    {
        [FormerlySerializedAs("_diamondsValueUi")] [SerializeField] private TMP_Text _talentPointsValueUi;

        private void OnEnable()
        {
            EventManager.I.OnGameStateChanged += OnGameStateChanged;
            EventManager.I.OnLeveledUp += OnLeveledUp;
        }
        
        private void OnDisable()
        {
            EventManager.I.OnGameStateChanged -= OnGameStateChanged;
            EventManager.I.OnLeveledUp -= OnLeveledUp;
        }
        
        private void OnGameStateChanged(EGameState state)
        {
            if (state is EGameState.Loaded)
                UpdateTalentPointsValueUi();
        }
        
        private void OnLeveledUp(int level)
        {
            AddTalentPoints(GameManager.Data.GetDiamondsForLeveledUp());
        }
        
        /// <summary>
        /// Adds talent points.
        /// </summary>
        /// <param name="amount"></param>
        private void AddTalentPoints(double amount)
        {
            if (amount == 0)
                return;
            
            GameManager.Data.TalentPointsCurrent += amount;
            UpdateTalentPointsValueUi();
            
            EventManager.I.TriggerTalentPointsUpdated(GameManager.Data.TalentPointsCurrent, amount);
        }
        
        /// <summary>
        /// Subtract talent points.
        /// </summary>
        /// <param name="amount"></param>
        private void SubtractTalentPoints(double amount)
        {
            if (amount == 0)
                return;
            
            GameManager.Data.TalentPointsCurrent -= amount;
            UpdateTalentPointsValueUi();
            
            EventManager.I.TriggerTalentPointsUpdated(GameManager.Data.TalentPointsCurrent, amount);
        }
        
        /// <summary>
        /// Returns if there is enough talent points.
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool CanAfford(double amount)
        {
            return GameManager.Data.TalentPointsCurrent >= amount;
        }
        
        /// <summary>
        /// Updates talent points value Ui.
        /// </summary>
        private void UpdateTalentPointsValueUi()
        {
            _talentPointsValueUi.SetText(Helper.GetNumberAsString(GameManager.Data.TalentPointsCurrent));
        }
    }
}