using System.Collections;
using Prez.V2.Enums;
using UnityEngine;

namespace Prez.V2.Managers
{
    public class GameStateManager : MonoBehaviour
    {
        public EGameState State { get; private set; }

        private EventManager _event;

        #region Bootstrap

        private void Awake()
        {
            _event = RefManager.Event;
        }

        private void OnEnable()
        {
            _event.OnGameStateChanged += OnGameStateChanged;
            _event.OnBricksLoaded += OnBricksLoaded;
        }

        private void OnDisable()
        {
            _event.OnGameStateChanged -= OnGameStateChanged;
            _event.OnBricksLoaded -= OnBricksLoaded;
        }

        #endregion

        #region Observers

        private void OnGameStateChanged(EGameState newState, EGameState oldState)
        {
            if (newState is EGameState.DataLoaded)
                SetState(EGameState.BricksLoading);
        }

        private void OnBricksLoaded()
        {
            SetState(EGameState.BricksLoaded);
            SetState(EGameState.Playing, 0f);
        }

        #endregion
        
        #region State

        /// <summary>
        /// Sets the state of the game.
        /// </summary>
        /// <param name="newState"></param>
        public void SetState(EGameState newState)
        {
            if (State == newState)
            {
                Debug.LogWarning($"Trying to set same state as current state: {newState}");
                return;
            }

            var oldState = newState;
            State = newState;
            
            _event.TriggerGameStateChanged(newState, oldState);
        }

        /// <summary>
        /// Sets the state of the game after a delay.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="delay"></param>
        public void SetState(EGameState state, float delay)
        {
            StartCoroutine(SetStateAfterDelay(state, delay));
        }

        /// <summary>
        /// Sets the state of the game after a delay.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        private IEnumerator SetStateAfterDelay(EGameState state, float delay)
        {
            if (delay <= 0f)
                yield return null;
            else
                yield return new WaitForSeconds(delay);

            SetState(state);
        }

        #endregion
    }
}