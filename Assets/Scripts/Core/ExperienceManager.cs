using System.Collections;
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
        
        private void OnEnable()
        {
            EventManager.I.OnGameStateChanged += OnGameStateChanged;
            EventManager.I.OnBrickDamaged += OnBrickDamaged;
            EventManager.I.OnBrickDestroyed += OnBrickDestroyed;
        }
        
        private void OnDisable()
        {
            EventManager.I.OnGameStateChanged -= OnGameStateChanged;
            EventManager.I.OnBrickDamaged -= OnBrickDamaged;
            EventManager.I.OnBrickDestroyed -= OnBrickDestroyed;
        }
        
        private void OnGameStateChanged(EGameState state)
        {
            if (state is EGameState.Loaded)
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
                
                if (GameManager.State is EGameState.Playing)
                    GameManager.Data.TimeCurrentLevel++;
            }
        }

        /// <summary>
        /// Adds damage experience.
        /// </summary>
        /// <param name="ball"></param>
        /// <param name="damage"></param>
        private void AddDamageExperience(Ball ball, double damage)
        {
            var experience = GameManager.Data.GetExperienceForBrickDamage(damage);

            if (ball.IsPlayerBoostActive)
                experience *= GameManager.Data.GetActivePlayExpMultiplier(ball);
            
            GameManager.Data.ExperienceCurrent += experience;
            UpdateExperienceUi();
            CheckLeveledUp();
        }
        
        /// <summary>
        /// Adds brick destroyed experience.
        /// </summary>
        /// <param name="health"></param>
        private void AddDestroyExperience(double health)
        {
            GameManager.Data.ExperienceCurrent += GameManager.Data.GetExperienceForBrickDestroyed(health);
            UpdateExperienceUi();
            CheckLeveledUp();
        }

        /// <summary>
        /// Check if leveld up.
        /// </summary>
        private void CheckLeveledUp()
        {
            if (GameManager.Data.ExperienceCurrent >= GameManager.Data.ExperienceRequiredToLevel)
                LevelUp();
        }

        /// <summary>
        /// Level up.
        /// </summary>
        private void LevelUp()
        {
            GameManager.Data.LevelCurrent++;
            MessageManager.Queue($"Level {GameManager.Data.LevelCurrent}!");
            SetLevelExperience(true);
            
            GameManager.Data.TimeTotal += GameManager.Data.TimeCurrentLevel;
            GameManager.Data.TimeCurrentLevel = 0d;
            
            EventManager.I.TriggerLeveledUp(GameManager.Data.LevelCurrent);
            UpdateExperienceUi();
        }
        
        /// <summary>
        /// Sets the current level experience.
        /// </summary>
        private void SetLevelExperience(bool leveledUp = false)
        {
            if (leveledUp)
                GameManager.Data.ExperienceCurrent -= GameManager.Data.ExperienceRequiredToLevel;
            
            GameManager.Data.ExperienceRequiredToLevel = GameManager.Data.GetExperienceNeededToLevel(GameManager.Data.LevelCurrent);
            
            UpdateLevelValueUi();
        }

        /// <summary>
        /// Updates the experience indicator Ui.
        /// </summary>
        private void UpdateExperienceUi()
        {
            var percent = GameManager.Data.ExperienceCurrent / GameManager.Data.ExperienceRequiredToLevel;

            _levelIndicator.DOKill(true);
            _levelIndicator.DOFillAmount((float)percent, 0.1f);
            _levelExperienceValueUi.SetText($"{Helper.GetNumberAsString(GameManager.Data.ExperienceCurrent)}\n{Helper.GetNumberAsString(GameManager.Data.ExperienceRequiredToLevel)}");
        }

        /// <summary>
        /// Updates the level value Ui.
        /// </summary>
        private void UpdateLevelValueUi()
        {
            _levelValueUi.SetText($"Level {GameManager.Data.LevelCurrent:0}");
        }
    }
}