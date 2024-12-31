using System.Collections;
using DG.Tweening;
using Prez.V1.Data;
using Prez.V1.Enums;
using Prez.V1.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VInspector;

namespace Prez.V1.Core
{
    public class ExperienceManager : MonoBehaviour
    {
        [Tab("General")] [SerializeField] private TMP_Text _levelValueUi;

        [SerializeField] private TMP_Text _levelExperienceValueUi;
        [SerializeField] private Image _levelIndicator;

        private void OnEnable()
        {
            EventManager.I.OnGameStateChanged += OnGameStateChanged;
            EventManager.I.OnBrickDamaged += OnBrickDamaged;
        }

        private void OnDisable()
        {
            EventManager.I.OnGameStateChanged -= OnGameStateChanged;
            EventManager.I.OnBrickDamaged -= OnBrickDamaged;
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

        private void OnBrickDamaged(DamageData data)
        {
            AddBrickDamagedExperience(data);
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
        ///     Adds brick damaged experience.
        /// </summary>
        /// <param name="data"></param>
        private void AddBrickDamagedExperience(DamageData data)
        {
            GameManager.Data.AddBrickDamagedExperience(data);

            if (data.Experience <= 0)
                return;

            // print($"Exp ({data.Brick.SpawnedRowNumber}) -> {data.Experience}");

            CheckLeveledUp();
            UpdateExperienceUi();
        }

        /// <summary>
        ///     Check if leveled up.
        /// </summary>
        private void CheckLeveledUp()
        {
            if (GameManager.Data.ExperienceCurrent >= GameManager.Data.ExperienceRequiredToLevel)
                LevelUp();
        }

        /// <summary>
        ///     Level up.
        /// </summary>
        private void LevelUp()
        {
            // print($"LevelUp ({GameManager.Data.LevelCurrent}) -> {GameManager.Data.TimeCurrentLevel}");

            GameManager.Data.LevelCurrent++;
            MessageManager.Queue($"Level {GameManager.Data.LevelCurrent}!");
            SetLevelExperience(true);

            GameManager.Data.TimeTotal += GameManager.Data.TimeCurrentLevel;
            GameManager.Data.TimeCurrentLevel = 0d;

            EventManager.I.TriggerLeveledUp(GameManager.Data.LevelCurrent);
            // UpdateExperienceUi();
        }

        /// <summary>
        ///     Sets the current level experience.
        /// </summary>
        private void SetLevelExperience(bool leveledUp = false)
        {
            if (leveledUp)
                GameManager.Data.ExperienceCurrent -= GameManager.Data.ExperienceRequiredToLevel;

            GameManager.Data.ExperienceRequiredToLevel = GameManager.Data.GetExperienceNeededToLevel(GameManager.Data.LevelCurrent);

            UpdateLevelValueUi();
        }

        /// <summary>
        ///     Updates the experience indicator Ui.
        /// </summary>
        private void UpdateExperienceUi()
        {
            var percent = GameManager.Data.ExperienceCurrent / GameManager.Data.ExperienceRequiredToLevel;

            _levelIndicator.DOKill(true);
            _levelIndicator.DOFillAmount((float)percent, 0.1f);
            _levelExperienceValueUi.SetText(
                $"{Helper.GetNumberAsString(GameManager.Data.ExperienceCurrent)}\n{Helper.GetNumberAsString(GameManager.Data.ExperienceRequiredToLevel)}");
        }

        /// <summary>
        ///     Updates the level value Ui.
        /// </summary>
        private void UpdateLevelValueUi()
        {
            _levelValueUi.SetText($"Level {GameManager.Data.LevelCurrent:0}");
        }
    }
}