using System;
using System.Collections;
using DG.Tweening;
using Prez.Data;
using Prez.Enums;
using UnityEngine;

namespace Prez.Core
{
    public class GameManager : MonoBehaviour
    {
        private EventManager _event;
        private GameData _gameData;
        private EGameState _state;
        public static GameManager I { get; private set; }
        public static EGameState State => I._state;
        public static GameData Data => I._gameData;

        private void Awake()
        {
            SetupSingleton();

            _event = EventManager.I;
            DOTween.SetTweensCapacity(500, 50);
        }

        private void Start()
        {
            SetState(EGameState.Loading);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.KeypadPlus))
                Time.timeScale = 10;

            if (Input.GetKeyDown(KeyCode.KeypadMinus))
                Time.timeScale = 1;
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
            StartCoroutine(SetDataAndStartGame(data));
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

        /// <summary>
        ///     Sets the game state.
        /// </summary>
        /// <param name="state"></param>
        private void SetState(EGameState state)
        {
            if (State == state)
                return;

            _state = state;
            _event.TriggerGameStateChanged(State);
        }

        private IEnumerator SetDataAndStartGame(GameData data)
        {
            _gameData = data;

            GenerateGameId();

            // Give the event time to trigger on all scripts
            yield return null;

            SetState(EGameState.Loaded);

            MessageManager.Queue("Spawning Bricks..", 1.6f);
            yield return new WaitForSeconds(0.1f);

            SetState(EGameState.Playing);
        }

        /// <summary>
        ///     Generate a new game id.
        /// </summary>
        private void GenerateGameId()
        {
            if (string.IsNullOrWhiteSpace(Data.GameId))
                Data.GameId = Guid.NewGuid().ToString();
        }
    }
}