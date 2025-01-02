using System.Collections;
using DG.Tweening;
using Prez.V2.Data;
using Prez.V2.Enums;
using Prez.V2.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Prez.V2.Managers
{
    public class ExperienceManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text _levelValueUi;
        [SerializeField] private TMP_Text _levelExperienceValueUi;
        [SerializeField] private TMP_Text _levelExperienceNeededValueUi;
        [SerializeField] private Image _levelIndicator;
        
        private EventManager _event;
        private GameManager _game;
        private MessageManager _message;

        #region Bootstrap

        private void Awake()
        {
            _event = RefManager.Event;
            _game = RefManager.Game;
            _message = RefManager.Message;
        }

        private void OnEnable()
        {
            _event.OnGameStateChanged += OnStateChanged;
            _event.OnBrickDamaged += OnBrickDamaged;
        }

        private void OnDisable()
        {
            _event.OnGameStateChanged -= OnStateChanged;
            _event.OnBrickDamaged -= OnBrickDamaged;
        }

        #endregion

        #region Observers

        private void OnStateChanged(EGameState newState, EGameState oldState)
        {
            if (newState is EGameState.DataLoaded)
            {
                UpdateLevelUi();
                UpdateExperienceNeededToLevel();
                UpdateExperienceUi();
                StartCoroutine(IncrementLevelTime());
            }
        }

        private void OnBrickDamaged(DamageData data)
        {
            AddExperience(data);
        }

        #endregion

        #region Experience

        /// <summary>
        /// Updates experience needed to level;
        /// </summary>
        private void UpdateExperienceNeededToLevel()
        {
            _game.Data.LevelData.ExperienceRequiredToLevel = _game.Data.LevelData.GetExperienceNeededToLevel();
        }
        
        /// <summary>
        /// Adds experience based on brick damage.
        /// </summary>
        /// <param name="data"></param>
        private void AddExperience(DamageData data)
        {
            data.Experience = _game.Data.LevelData.GetExperienceForBrickDamage(data);
            _game.Data.LevelData.Experience += data.Experience;

            _event.TriggerExperienceGained(data.Experience);

            if (_game.Data.LevelData.Experience >= _game.Data.LevelData.ExperienceRequiredToLevel)
                LevelUp();
            
            UpdateExperienceUi();
        }

        /// <summary>
        /// Level up.
        /// </summary>
        private void LevelUp()
        {
            _game.Data.LevelData.Experience -= _game.Data.LevelData.ExperienceRequiredToLevel;
            _game.Data.LevelData.Level++;
            
            UpdateExperienceNeededToLevel();
            UpdateLevelUi();
            IncrementTotalTime();
            
            _event.TriggerLeveledUp(_game.Data.LevelData.Level);
            _message.Queue($"Level {_game.Data.LevelData.Level}!");
        }

        #endregion

        #region Time

        /// <summary>
        /// Increments the level time.
        /// </summary>
        /// <returns></returns>
        private IEnumerator IncrementLevelTime()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);

                if (_game.State is EGameState.Playing)
                    _game.Data.LevelData.TimeThisLevel++;
            }
        }

        /// <summary>
        /// Increments the total time
        /// and resets the level time.
        /// </summary>
        private void IncrementTotalTime()
        {
            _game.Data.LevelData.TimeTotal += _game.Data.LevelData.TimeThisLevel;
            _game.Data.LevelData.TimeThisLevel = 0;
        }

        #endregion

        #region Ui

        /// <summary>
        /// Updates the level value Ui.
        /// </summary>
        private void UpdateLevelUi()
        {
            _levelValueUi.SetText($"Level {_game.Data.LevelData.Level}");
        }
        
        /// <summary>
        /// Updates the experience indicator Ui.
        /// </summary>
        private void UpdateExperienceUi()
        {
            var percent = _game.Data.LevelData.Experience / _game.Data.LevelData.ExperienceRequiredToLevel;

            _levelIndicator.DOKill(true);
            _levelIndicator.DOFillAmount((float)percent, 0.1f);
            _levelExperienceValueUi.SetText(Helper.GetNumberAsString(_game.Data.LevelData.Experience));
            _levelExperienceNeededValueUi.SetText(Helper.GetNumberAsString(_game.Data.LevelData.ExperienceRequiredToLevel));
        }

        #endregion
    }
}