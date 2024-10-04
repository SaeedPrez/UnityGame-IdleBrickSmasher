using DG.Tweening;
using Prez.Data;
using Prez.Enums;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Prez.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager I { get; private set; }
        public GameData Data { get; private set; } = new();
        public EGameState State { get; private set; }

        private void Awake()
        {
            SetupSingleton();
            DOTween.SetTweensCapacity(500, 50);
        }
        
        private void Start()
        {
            SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
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

        private void SetState(EGameState state)
        {
            if (State == state)
                return;

            State = state;
        }
    }
}