using System;
using Data;
using Enums;
using Menus;
using UnityEngine;

namespace Core
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

        public event Action<Brick, Ball, double, bool> OnBrickDamaged = delegate { };
        public void TriggerBrickDamaged(Brick brick, Ball ball, double damage, bool destroyed) => OnBrickDamaged?.Invoke(brick, ball, damage, destroyed);
        
        // Experience & Level

        public event Action<int> OnLeveledUp = delegate { };
        public void TriggerLeveledUp(int level) => OnLeveledUp?.Invoke(level);
        
        // Coins

        public event Action<double, double> OnCoinsUpdated = delegate { };
        public void TriggerCoinsUpdated(double total, double change) => OnCoinsUpdated?.Invoke(total, change);
        
        // Diamonds
        
        public event Action<double> OnDiamondsGained = delegate { };
        public void TriggerDiamondsGained(double amount) => OnDiamondsGained?.Invoke(amount);

        public event Action<double> OnDiamondsUsed = delegate { };
        public void TriggerDiamondsUsed(double amount) => OnDiamondsUsed?.Invoke(amount);
        
        // Balls

        public event Action<BallMenuRow> OnBallMenuRowUnlocked = delegate { };
        public void TriggerBallMenuRowUnlocked(BallMenuRow ballMenuRow) => OnBallMenuRowUnlocked?.Invoke(ballMenuRow);
        
        public event Action<Ball> OnBallCollidedWithPlayer = delegate { };
        public void TriggerBallCollidedWithPlayer(Ball ball) => OnBallCollidedWithPlayer?.Invoke(ball);
        
        public event Action<Ball, Brick, Vector2> OnBallCollidedWithBrick = delegate { };
        public void TriggerBallCollidedWithBrick(Ball ball, Brick brick, Vector2 point) =>
            OnBallCollidedWithBrick?.Invoke(ball, brick, point);
        
        public event Action<Ball> OnBallCollidedWithBottomWall = delegate { };
        public void TriggerBallCollidedWithBottomWall(Ball ball) => OnBallCollidedWithBottomWall?.Invoke(ball);

        public event Action<Ball> OnBallRequestRespawn = delegate { };
        public void TriggerBallRequestRespawn(Ball ball) => OnBallRequestRespawn?.Invoke(ball);

        public event Action<Ball, EStat> OnBallUpgraded = delegate { };
        public void TriggerBallUpgraded(Ball ball, EStat stat) => OnBallUpgraded?.Invoke(ball, stat);
        
        // Menus

        public event Action<MenuBase> OnBottomMenuHidden = delegate { };
        public void TriggerBottomMenuHidden(MenuBase menu) => OnBottomMenuHidden?.Invoke(menu);
        
        
        public static EventManager I { get; private set; }

        private void Awake()
        {
            SetupSingleton();
        }

        /// <summary>
        /// Setups the singleton.
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
    }
}