using System;
using UnityEngine;

namespace Prez.Core
{
    public class EventManager : MonoBehaviour
    {
        public event Action<Vector2> OnPlayerInputMove = delegate { };
        public void RaisePlayerInput(Vector2 playerInput) => OnPlayerInputMove(playerInput);

        public event Action OnPlayerInputReleaseBall = delegate { };
        public void RaisePlayerInputReleaseBall() => OnPlayerInputReleaseBall();
        
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