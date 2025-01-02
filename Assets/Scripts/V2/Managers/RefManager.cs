using UnityEngine;

namespace Prez.V2.Managers
{
    public class RefManager : MonoBehaviour
    {
        [SerializeField] private EventManager _eventManager;
        [SerializeField] private GameManager _gameManager;
        [SerializeField] private MessageManager _messageManager;

        public static EventManager Event => I._eventManager;
        public static GameManager Game => I._gameManager;
        public static MessageManager Message => I._messageManager;
        
        public static RefManager I { get; private set; }

        #region Bootstrap

        private void Awake()
        {
            SetupSingleton();
        }

        #endregion

        #region Singleton

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

        #endregion
    }
}