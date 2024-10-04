using Prez.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Prez.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager I { get; private set; }
        public GameData Data { get; private set; } = new();

        private void Awake()
        {
            SetupSingleton();
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
    }
}