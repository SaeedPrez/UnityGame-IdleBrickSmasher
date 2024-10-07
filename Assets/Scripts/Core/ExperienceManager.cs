using System.Collections;
using DG.Tweening;
using Prez.Data;
using Prez.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Prez.Core
{
    public class ExperienceManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text _levelValueUi;
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

        private void OnBrickDestroyed(Brick brick, long maxHealth)
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

        private void AddExperience(long maxHealth)
        {
            _gameData.ExperienceCurrent.Add(_gameData.GetExperience(maxHealth));

            if (_gameData.ExperienceCurrent.AsLong < _gameData.ExperienceRequiredToLevel.AsLong)
                return;

            LevelUp();
        }

        private void LevelUp()
        {
            _gameData.Level++;
            SetLevelExperience();
            
            _gameData.TimeTotal.Add(_gameData.TimeThisLevel);
            _gameData.TimeThisLevel = 0;
            
            _event.TriggerLeveledUp(_gameData.Level);
        }

        private void SetLevelExperience()
        {
            _gameData.ExperienceCurrent.Set(_gameData.ExperienceCurrent.AsLong - _gameData.ExperienceRequiredToLevel.AsLong);
            _gameData.ExperienceRequiredToLevel.Set(_gameData.GetExperienceNeededToLevel(_gameData.Level));
            
            UpdateLevelValueUi();
        }

        private void UpdateExperienceIndicatorUi()
        {
            var percent = _gameData.ExperienceCurrent.AsLong / (float)_gameData.ExperienceRequiredToLevel.AsLong;

            _levelIndicator.DOKill();
            _levelIndicator.DOFillAmount(percent, 0.1f);
        }

        private void UpdateLevelValueUi()
        {
            _levelValueUi.text = $"Level {_gameData.Level}";
        }
    }
}