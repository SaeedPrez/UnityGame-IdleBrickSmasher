using System.Collections;
using Data;
using DG.Tweening;
using Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using VInspector;

namespace Core
{
    public class ExperienceManager : MonoBehaviour
    {
        [Tab("General")]
        
        [SerializeField] private TMP_Text _levelValueUi;
        [SerializeField] private TMP_Text _levelExperienceValueUi;
        [SerializeField] private Image _levelIndicator;
        
        private EventManager _event;
        private GameManager _game;
        private GameData _gameData;

        private void Awake()
        {
            _event = EventManager.I;
            _game = GameManager.I;
            _gameData = _game.Data;
        }

        private void OnEnable()
        {
            _event.OnGameStateChanged += OnGameStateChanged;
            _event.OnBrickDamaged += OnBrickDamaged;
            _event.OnBrickDestroyed += OnBrickDestroyed;
        }
        
        private void OnDisable()
        {
            _event.OnGameStateChanged -= OnGameStateChanged;
            _event.OnBrickDestroyed -= OnBrickDestroyed;
        }
        
        private void OnGameStateChanged(EGameState state)
        {
            if (state is EGameState.NewGame or EGameState.ContinueGame)
            {
                StartCoroutine(IncrementTime());
                SetLevelExperience();
                UpdateExperienceUi();
            }
        }
        
        private void OnBrickDamaged(Brick brick, Ball ball, double damage)
        {
            AddDamageExperience(ball, damage);
        }

        private void OnBrickDestroyed(Brick brick, double health)
        {
            AddDestroyExperience(health);
        }

        private IEnumerator IncrementTime()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                
                if (_game.State is EGameState.Playing)                
                    _gameData.TimeThisLevel++;
            }
        }

        /// <summary>
        /// Adds damage experience.
        /// </summary>
        /// <param name="ball"></param>
        /// <param name="damage"></param>
        private void AddDamageExperience(Ball ball, double damage)
        {
            var experience = _gameData.GetExpForDamage(damage);

            if (ball.IsPlayerBoostActive)
                experience *= _gameData.GetActivePlayExpMultiplier(ball);
            
            _gameData.ExperienceCurrent += experience;
            UpdateExperienceUi();
            CheckLeveledUp();
        }
        
        /// <summary>
        /// Adds brick destroyed experience.
        /// </summary>
        /// <param name="health"></param>
        private void AddDestroyExperience(double health)
        {
            _gameData.ExperienceCurrent += _gameData.GetExpForDestroyed(health);
            UpdateExperienceUi();
            CheckLeveledUp();
        }

        /// <summary>
        /// Check if leveld up.
        /// </summary>
        private void CheckLeveledUp()
        {
            if (_gameData.ExperienceCurrent >= _gameData.ExperienceRequiredToLevel)
                LevelUp();
        }

        /// <summary>
        /// Level up.
        /// </summary>
        private void LevelUp()
        {
            _gameData.Level++;
            SetLevelExperience();
            
            _gameData.TimeTotal += _gameData.TimeThisLevel;
            _gameData.TimeThisLevel = 0d;
            
            _event.TriggerLeveledUp(_gameData.Level);
            UpdateExperienceUi();
        }

        /// <summary>
        /// Sets the current level experience.
        /// </summary>
        private void SetLevelExperience()
        {
            _gameData.ExperienceCurrent -= _gameData.ExperienceRequiredToLevel;
            _gameData.ExperienceRequiredToLevel = _gameData.GetExpNeededToLevel(_gameData.Level);
            
            UpdateLevelValueUi();
        }

        /// <summary>
        /// Updates the experience indicator Ui.
        /// </summary>
        private void UpdateExperienceUi()
        {
            var percent = _gameData.ExperienceCurrent / _gameData.ExperienceRequiredToLevel;

            _levelIndicator.DOKill(true);
            _levelIndicator.DOFillAmount((float)percent, 0.1f);
            _levelExperienceValueUi.SetText($"{Helper.GetNumberAsString(_gameData.ExperienceCurrent)}\n{Helper.GetNumberAsString(_gameData.ExperienceRequiredToLevel)}");
        }

        /// <summary>
        /// Updates the level value Ui.
        /// </summary>
        private void UpdateLevelValueUi()
        {
            _levelValueUi.SetText($"Level {_gameData.Level:0}");
        }
    }
}