using System;
using Prez.Enums;
using UnityEngine;

namespace Prez.Core
{
    public class EventManager : MonoBehaviour
    {
        // Game

        public event Action<EGameState> OnGameStateChanged = delegate { };
        public void TriggerGameStateChanged(EGameState state) => OnGameStateChanged?.Invoke(state);
        
        // Input
        
        public event Action<Vector2> OnPlayerInputMove = delegate { };
        public void TriggerPlayerInput(Vector2 playerInput) => OnPlayerInputMove?.Invoke(playerInput);

        public event Action OnPlayerInputBallAction1 = delegate { };
        public void TriggerPlayerBallAction1() => OnPlayerInputBallAction1?.Invoke();

        // Bricks
        
        public event Action<Brick, double> OnBrickDestroyed = delegate { };
        public void TriggerBrickDestroyed(Brick brick, double maxHealth) => OnBrickDestroyed?.Invoke(brick, maxHealth);
        
        // Experience & Level

        public event Action<double> OnLeveledUp = delegate { };
        public void TriggerLeveledUp(double level) => OnLeveledUp?.Invoke(level);
        
        // Coins

        public event Action<double> OnCoinsGained = delegate { };
        public void TriggerCoinsGained(double amount) => OnCoinsGained?.Invoke(amount);

        public event Action<double> OnCoinsUsed = delegate { };
        public void TriggerCoinsUsed(double amount) => OnCoinsUsed?.Invoke(amount);

        // Diamonds
        
        public event Action<double> OnDiamondsGained = delegate { };
        public void TriggerDiamondsGained(double amount) => OnDiamondsGained?.Invoke(amount);

        public event Action<double> OnDiamondsUsed = delegate { };
        public void TriggerDiamondsUsed(double amount) => OnDiamondsUsed?.Invoke(amount);
        
        
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