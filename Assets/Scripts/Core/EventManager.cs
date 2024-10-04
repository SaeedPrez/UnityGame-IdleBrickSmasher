using System;
using UnityEngine;

namespace Prez.Core
{
    public class EventManager : MonoBehaviour
    {
        public event Action<Vector2> OnPlayerInputMove = delegate { };
        public void RaisePlayerInput(Vector2 playerInput) => OnPlayerInputMove.Invoke(playerInput);

        public event Action OnPlayerInputBallAction1 = delegate { };
        public void TriggerPlayerBallAction1() => OnPlayerInputBallAction1.Invoke();

        public event Action<Brick> OnBrickDestroyed = delegate { };
        public void TriggerBrickDestroyed(Brick brick) => OnBrickDestroyed.Invoke(brick);
        
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