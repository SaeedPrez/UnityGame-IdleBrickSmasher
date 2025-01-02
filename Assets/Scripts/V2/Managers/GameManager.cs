using Prez.V2.Data.Save;
using Prez.V2.Enums;
using UnityEngine;

namespace Prez.V2.Managers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameStateManager _gameState;

        public EGameState State => _gameState.State;
        public GameData Data { get; private set; } = new GameData();

        #region Bootstrap

        private void Start()
        {
            Data.Reset();
            _gameState.SetState(EGameState.DataLoading);
            _gameState.SetState(EGameState.DataLoaded, 0.1f);
        }

        #endregion
    }
}