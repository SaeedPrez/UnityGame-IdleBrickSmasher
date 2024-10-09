using System.Collections;
using DG.Tweening;
using Prez.Data;
using Prez.Enums;
using Prez.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Prez.Core
{
    public class ExperienceManager : MonoBehaviour
    {
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
                UpdateExperienceIndicatorUi();
            }
        }

        private void OnBrickDestroyed(Brick brick, double maxHealth)
        {
            AddExperience(maxHealth);
            UpdateExperienceIndicatorUi();
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

        private void AddExperience(double maxHealth)
        {
            _gameData.ExperienceCurrent += _gameData.GetExperienceGainedPerHealth(maxHealth);

            if (_gameData.ExperienceCurrent < _gameData.ExperienceRequiredToLevel)
                return;

            LevelUp();
        }

        private void LevelUp()
        {
            _gameData.Level++;
            SetLevelExperience();
            
            _gameData.TimeTotal += _gameData.TimeThisLevel;
            _gameData.TimeThisLevel = 0d;
            
            _event.TriggerLeveledUp(_gameData.Level);
        }

        private void SetLevelExperience()
        {
            _gameData.ExperienceCurrent -= _gameData.ExperienceRequiredToLevel;
            _gameData.ExperienceRequiredToLevel = _gameData.GetExperienceNeededToLevel(_gameData.Level);
            
            UpdateLevelValueUi();
        }

        private void UpdateExperienceIndicatorUi()
        {
            var percent = _gameData.ExperienceCurrent / _gameData.ExperienceRequiredToLevel;

            _levelIndicator.DOKill(true);
            _levelIndicator.DOFillAmount((float)percent, 0.1f);
            _levelExperienceValueUi.SetText($"{Helper.GetNumberAsString(_gameData.ExperienceCurrent)}\n{Helper.GetNumberAsString(_gameData.ExperienceRequiredToLevel)}");
        }

        private void UpdateLevelValueUi()
        {
            _levelValueUi.SetText($"Level {_gameData.Level:0}");
        }
    }
}