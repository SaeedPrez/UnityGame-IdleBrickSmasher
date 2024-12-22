using System;
using Prez.Data;
using Prez.Enums;
using Prez.Menus;
using UnityEngine;

namespace Prez.Core
{
    public class EventManager : MonoBehaviour
    {
        // Game

        public event Action<EGameState> OnGameStateChanged = delegate { };
        public void TriggerGameStateChanged(EGameState state) => OnGameStateChanged?.Invoke(state);

        public event Action<GameData> OnGameDataLoaded = delegate { };
        public void TriggerGameDataLoaded(GameData data) => OnGameDataLoaded?.Invoke(data);

        public event Action<GameData> OnGameDataSaved = delegate { };
        public void TriggerGameDataSaved(GameData data) => OnGameDataSaved?.Invoke(data);

        // Input

        public event Action<Vector2> OnPlayerInputMove = delegate { };
        public void TriggerPlayerInput(Vector2 playerInput) => OnPlayerInputMove?.Invoke(playerInput);

        public event Action OnPlayerInputBallAction1 = delegate { };
        public void TriggerPlayerBallAction1() => OnPlayerInputBallAction1?.Invoke();

        // Bricks

        public event Action<DamageData> OnBrickDamaged = delegate { };
        public void TriggerBrickDamaged(DamageData data) => OnBrickDamaged?.Invoke(data);

        // Experience & Level

        public event Action<int> OnLeveledUp = delegate { };
        public void TriggerLeveledUp(int level) => OnLeveledUp?.Invoke(level);

        // Upgrade Points

        public event Action<double, double> OnUpgradePointsUpdated = delegate { };
        public void TriggerUpgradePointsUpdated(double total, double change) => OnUpgradePointsUpdated?.Invoke(total, change);

        // Talents Points

        public event Action<double, double> OnTalentPointsUpdated = delegate { };
        public void TriggerTalentPointsUpdated(double total, double amount) => OnTalentPointsUpdated?.Invoke(total, amount);

        // Paddle
        
        public event Action<EStat, float> OnPaddleUpgraded = delegate { };
        public void TriggerPaddleUpgraded(EStat stat, float cost) => OnPaddleUpgraded?.Invoke(stat, cost);
        
        public event Action<EStat, float> OnPaddleBulletUpgraded = delegate { };
        public void TriggerPaddleBulletUpgraded(EStat stat, float cost) => OnPaddleBulletUpgraded?.Invoke(stat, cost);
        
        // Balls

        public event Action<BallMenuRow> OnBallMenuRowUnlocked = delegate { };
        public void TriggerBallMenuRowUnlocked(BallMenuRow ballMenuRow) => OnBallMenuRowUnlocked?.Invoke(ballMenuRow);

        public event Action<Ball> OnBallCollidedWithPlayer = delegate { };
        public void TriggerBallCollidedWithPlayer(Ball ball) => OnBallCollidedWithPlayer?.Invoke(ball);

        public event Action<Ball, Brick, Vector2> OnBallCollidedWithBrick = delegate { };
        public void TriggerBallCollidedWithBrick(Ball ball, Brick brick, Vector2 point) => OnBallCollidedWithBrick?.Invoke(ball, brick, point);

        public event Action<Ball> OnBallCollidedWithBottomWall = delegate { };
        public void TriggerBallCollidedWithBottomWall(Ball ball) => OnBallCollidedWithBottomWall?.Invoke(ball);

        public event Action<Ball> OnBallRequestRespawn = delegate { };
        public void TriggerBallRequestRespawn(Ball ball) => OnBallRequestRespawn?.Invoke(ball);

        public event Action<Ball, EStat, float> OnBallUpgraded = delegate { };
        public void TriggerBallUpgraded(Ball ball, EStat stat, float cost) => OnBallUpgraded?.Invoke(ball, stat, cost);

        // Bullets
        
        public event Action<Brick, Vector2> OnBulletCollidedWithBrick = delegate { };
        public void TriggerBulletCollidedWithBrick(Brick brick, Vector2 point) => OnBulletCollidedWithBrick?.Invoke(brick, point);

        // Menus

        public event Action<MenuBase> OnBottomMenuHidden = delegate { };
        public void TriggerBottomMenuHidden(MenuBase menu) => OnBottomMenuHidden?.Invoke(menu);

        #region Singleton

        public static EventManager I { get; private set; }

        private void Awake()
        {
            SetupSingleton();
        }
        
        /// <summary>
        ///     Setups the singleton.
        /// </summary>
        private void SetupSingleton()
        {
            if (I)
            {
                Destroy(gameObject);
                return;
            }

            I = this;
        }

        #endregion
    }
}