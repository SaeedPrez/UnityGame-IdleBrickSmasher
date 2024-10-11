using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using Enums;
using Menus;
using UnityEngine;

namespace Core
{
    public class BallManager : MonoBehaviour
    {
        [SerializeField] private Ball _ballPrefab;
        [SerializeField] private BallMenuRow _ballMenuRowPrefab;
        [SerializeField] private RectTransform _ballMenuRowContainer;
        [SerializeField] private Transform _ballContainer;
        [SerializeField] private ParticleSystem _ballSpawnEffect;
        
        private EventManager _event;
        private GameManager _game;
        private GameData _gameData;
        private List<BallMenuRow> _ballMenuRows = new();
        
        private void Awake()
        {
            _event = EventManager.I;
            _game = GameManager.I;
            _gameData = _game.Data;
        }

        private void OnEnable()
        {
            _event.OnGameStateChanged += OnGameStateChanged;
            _event.OnBallCollidedWithPlayer += OnBallCollidedWithPlayer;
            _event.OnBallCollidedWithBrick += OnBallCollidedWithBrick;
            _event.OnBallCollidedWithBottomWall += OnBallCollidedWithBottomWall;
            _event.OnLeveledUp += OnLeveledUp;
        }
        
        private void OnDisable()
        {
            _event.OnGameStateChanged -= OnGameStateChanged;
            _event.OnBallCollidedWithPlayer -= OnBallCollidedWithPlayer;
            _event.OnBallCollidedWithBrick -= OnBallCollidedWithBrick;
            _event.OnBallCollidedWithBottomWall -= OnBallCollidedWithBottomWall;
            _event.OnLeveledUp -= OnLeveledUp;
        }
        
        private void OnGameStateChanged(EGameState state)
        {
            if (state is EGameState.NewGame or EGameState.ContinueGame)
                StartCoroutine(CreateBallMenuRows());
        }

        private void OnBallCollidedWithPlayer(Ball ball)
        {
            ball.EnableActivePlayBoost();
        }

        private void OnBallCollidedWithBrick(Ball ball, Brick brick)
        {
            DamageBrick(ball, brick);
        }

        private void OnBallCollidedWithBottomWall(Ball ball)
        {
            ball.DisableActivePlayBoost();
        }

        private void OnLeveledUp(int level)
        {
            var ballMenuRow = _ballMenuRows.FirstOrDefault(bmr => bmr.Ball.Data.UnlockLevel == level);
            
            if (ballMenuRow)
                UnlockBallMenuRow(ballMenuRow);
        }

        #region Ball Menu Rows

        /// <summary>
        /// Creates ball menu rows and their balls.
        /// </summary>
        private IEnumerator CreateBallMenuRows()
        {
            foreach (var ballData in _gameData.Balls)
            {
                var ball = Instantiate(_ballPrefab, _ballContainer);
                var ballMenuRow = Instantiate(_ballMenuRowPrefab, _ballMenuRowContainer);
                _ballMenuRows.Add(ballMenuRow);
                ballMenuRow.SetBall(ball);
                ballMenuRow.SetData(ballData);
            }

            yield return new WaitForSeconds(0.2f);
            
            foreach (var ballMenuRow in _ballMenuRows)
            {
                if (ballMenuRow.Data.UnlockLevel <= _gameData.Level)
                {
                    ballMenuRow.gameObject.SetActive(true);
                    yield return new WaitForSeconds(0.15f);
                    UnlockBallMenuRow(ballMenuRow);
                }
                else
                {
                    ballMenuRow.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Set a randomized ball velocity.
        /// </summary>
        /// <param name="ball"></param>
        private void RandomizeBallVelocity(Ball ball)
        {
            ball.SetRandomDirectionVelocity();
        }

        /// <summary>
        /// Unlocks ball menu row.
        /// </summary>
        /// <param name="ballMenuRow"></param>
        private void UnlockBallMenuRow(BallMenuRow ballMenuRow)
        {
            if (ballMenuRow.IsUnlocked)
                return;
            
            _ballSpawnEffect.Play();
            ballMenuRow.Unlock();
            RandomizeBallVelocity(ballMenuRow.Ball);
            EnableNextBallMenuRow();
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
        
        #region Bricks

        /// <summary>
        /// Damages brick and reduces balls active player boost.
        /// </summary>
        /// <param name="ball"></param>
        /// <param name="brick"></param>
        private void DamageBrick(Ball ball, Brick brick)
        {
            var damage = _gameData.GetBallDamage(ball);

            if (ball.IsPlayerBoostActive)
                damage *= _gameData.GetActivePlayDamageMultiplier(ball);

            brick.TakeDamage(ball, damage);
            ball.ReduceActivePlayBoostHits();
            ball.Data.TotalDamage += damage;
        }

        #endregion
    }
}