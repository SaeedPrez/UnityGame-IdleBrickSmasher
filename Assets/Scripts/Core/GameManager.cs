using System.Collections;
using Data;
using DG.Tweening;
using Enums;
using UnityEngine;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager I { get; private set; }
        public static EGameState State => I._state;
        public static GameData Data => I._gameData;

        private EventManager _event;
        private EGameState _state;
        private GameData _gameData;
        
        private void Awake()
        {
            SetupSingleton();

            _event = EventManager.I;
            DOTween.SetTweensCapacity(500, 50);
        }

        private void OnEnable()
        {
            _event.OnGameDataLoaded += OnGameDataLoaded;
        }

        private void OnDisable()
        {
            _event.OnGameDataLoaded -= OnGameDataLoaded;
        }

        private void OnGameDataLoaded(GameData data)
        {
            StartCoroutine(GameDataLoaded(data));
        }
        
        private void Start()
        {
            SetState(EGameState.Loading);
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

            _state = state;
            _event.TriggerGameStateChanged(State);
        }

        private IEnumerator GameDataLoaded(GameData data)
        {
            _gameData = data;
            
            // Give the event time to trigger on all scripts
            yield return null;

            SetState(EGameState.Loaded);

            yield return new WaitForSeconds(0.1f);
            
            SetState(EGameState.Playing);
        }
    }
}