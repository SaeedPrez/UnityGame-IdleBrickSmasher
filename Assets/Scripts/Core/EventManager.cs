using System;
using Prez.Data;
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
        
        public event Action<Brick, long> OnBrickDestroyed = delegate { };
        public void TriggerBrickDestroyed(Brick brick, long maxHealth) => OnBrickDestroyed?.Invoke(brick, maxHealth);
        
        // Experience & Level

        public event Action<uint> OnLeveledUp = delegate { };
        public void TriggerLeveledUp(uint level) => OnLeveledUp?.Invoke(level);
        
        // Coins

        public event Action<long> OnCoinsGained = delegate { };
        public void TriggerCoinsGained(long amount) => OnCoinsGained?.Invoke(amount);

        public event Action<long> OnCoinsUsed = delegate { };
        public void TriggerCoinsUsed(long amount) => OnCoinsUsed?.Invoke(amount);

        
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