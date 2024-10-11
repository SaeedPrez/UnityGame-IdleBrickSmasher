using Data;
using DG.Tweening;
using Enums;
using UnityEngine;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager I { get; private set; }
        public GameData Data { get; private set; } = new();
        public EGameState State { get; private set; }

        private EventManager _event;
        
        private void Awake()
        {
            SetupSingleton();

            _event = EventManager.I;
            DOTween.SetTweensCapacity(500, 50);
        }
        
        private void Start()
        {
            SetState(EGameState.NewGame);
            SetState(EGameState.Playing);
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

        /// <summary>
        /// Sets the game state.
        /// </summary>
        /// <param name="state"></param>
        private void SetState(EGameState state)
        {
            if (State == state)
                return;

            State = state;
            _event.TriggerGameStateChanged(State);
        }
    }
}