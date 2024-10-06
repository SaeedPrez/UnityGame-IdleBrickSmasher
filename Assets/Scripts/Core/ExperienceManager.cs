using DG.Tweening;
using Prez.Data;
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
        private GameData _data;

        private void Awake()
        {
            _event = EventManager.I;
            _data = GameManager.I.Data;
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
            AddExperience(maxHealth);
            UpdateExperienceIndicatorUi();
        }

        private void AddExperience(long maxHealth)
        {
            _data.ExperienceCurrent.Add(maxHealth * _data.ExperiencePerHealthBase);

            if (_data.ExperienceCurrent.AsLong < _data.ExperienceRequiredToLevel.AsLong)
                return;
            
            _data.ExperienceCurrent.Set(_data.ExperienceCurrent.AsLong - _data.ExperienceRequiredToLevel.AsLong);
            _data.Level++;

            _data.ExperienceRequiredToLevel.Set((long)GetExperienceNeededToLevel(_data.Level));
            UpdateLevelValueUi();
            
            _event.TriggerLeveledUp(_data.Level);
        }

        private float GetExperienceNeededToLevel(uint level)
        {
            // TODO: Handle bigger numbers than float.
            return _data.ExperienceLevelBase + Mathf.Pow(_data.ExperienceLevelGrowth, level);
        }

        private void UpdateExperienceIndicatorUi()
        {
            var percent = _data.ExperienceCurrent.AsLong / (float)_data.ExperienceRequiredToLevel.AsLong;

            _levelIndicator.DOKill();
            _levelIndicator.DOFillAmount(percent, 0.1f);
        }

        private void UpdateLevelValueUi()
        {
            _levelValueUi.text = $"Level {_data.Level}";
        }
    }
}