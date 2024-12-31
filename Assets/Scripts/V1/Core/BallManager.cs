﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Prez.V1.Enums;
using Prez.V1.Menus;
using UnityEngine;

namespace Prez.V1.Core
{
    public class BallManager : MonoBehaviour
    {
        [SerializeField] private Ball _ballPrefab;
        [SerializeField] private BallMenuRow _ballMenuRowPrefab;
        [SerializeField] private RectTransform _ballMenuRowContainer;
        [SerializeField] private Transform _ballContainer;
        [SerializeField] private ParticleSystem _ballSpawnEffect;

        private readonly List<BallMenuRow> _ballMenuRows = new();

        private void OnEnable()
        {
            EventManager.I.OnGameStateChanged += OnGameStateChanged;
            EventManager.I.OnLeveledUp += OnLeveledUp;
            EventManager.I.OnBallRequestRespawn += OnBallRequestRespawn;
        }

        private void OnDisable()
        {
            EventManager.I.OnGameStateChanged -= OnGameStateChanged;
            EventManager.I.OnLeveledUp -= OnLeveledUp;
            EventManager.I.OnBallRequestRespawn -= OnBallRequestRespawn;
        }

        private void OnGameStateChanged(EGameState state)
        {
            if (state is EGameState.Loaded)
                StartCoroutine(CreateBallMenuRows());
        }

        private void OnLeveledUp(int level)
        {
            var ballMenuRow = _ballMenuRows.FirstOrDefault(bmr => bmr.Ball.Data.UnlockLevel == level);

            if (!ballMenuRow)
                return;

            UnlockBallMenuRow(ballMenuRow);
            MessageManager.Queue($"Ball {ballMenuRow.Ball.Data.Id} unlocked!");
        }

        private void OnBallRequestRespawn(Ball ball)
        {
            SpawnBall(ball);
        }

        #region Ball Menu Rows

        /// <summary>
        ///     Creates ball menu rows and their balls.
        /// </summary>
        private IEnumerator CreateBallMenuRows()
        {
            foreach (var ballData in GameManager.Data.Balls)
            {
                var ball = Instantiate(_ballPrefab, _ballContainer);
                var ballMenuRow = Instantiate(_ballMenuRowPrefab, _ballMenuRowContainer);
                _ballMenuRows.Add(ballMenuRow);
                ballMenuRow.SetBall(ball);
                ballMenuRow.SetData(ballData);
                ball.gameObject.SetActive(false);
            }

            yield return new WaitForSeconds(1.5f);

            foreach (var ballMenuRow in _ballMenuRows)
                if (ballMenuRow.Data.UnlockLevel <= GameManager.Data.LevelCurrent)
                {
                    ballMenuRow.gameObject.SetActive(true);
                    yield return new WaitForSeconds(0.15f);
                    UnlockBallMenuRow(ballMenuRow);
                }
        }

        /// <summary>
        ///     Unlocks ball menu row.
        /// </summary>
        /// <param name="ballMenuRow"></param>
        private void UnlockBallMenuRow(BallMenuRow ballMenuRow)
        {
            if (ballMenuRow.IsUnlocked)
                return;

            ballMenuRow.Unlock();
            EnableNextBallMenuRow();
            SpawnBall(ballMenuRow.Ball);
        }

        /// <summary>
        ///     Spawns a ball.
        /// </summary>
        /// <param name="ball"></param>
        private void SpawnBall(Ball ball)
        {
            ball.transform.localPosition = Vector3.zero;
            ball.gameObject.SetActive(true);
            RandomizeBallVelocity(ball);
            _ballSpawnEffect.Play();
        }

        /// <summary>
        ///     Set a randomized ball velocity.
        /// </summary>
        /// <param name="ball"></param>
        private void RandomizeBallVelocity(Ball ball)
        {
            ball.SetRandomDirectionVelocity();
        }

        private void EnableNextBallMenuRow()
        {
            var nextBallMenuRow = _ballMenuRows
                .OrderBy(bmr => bmr.Data.UnlockLevel)
                .FirstOrDefault(bmr => !bmr.IsUnlocked);

            if (nextBallMenuRow)
                nextBallMenuRow.gameObject.SetActive(true);
        }

        #endregion
    }
}