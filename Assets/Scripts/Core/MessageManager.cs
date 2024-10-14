using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Enums;
using TMPro;
using UnityEngine;

namespace Core
{
    public class MessageManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text _messageValue;
        [SerializeField] private float _animationDuration;

        public static MessageManager I { get; private set; }
        
        private readonly Dictionary<string, float> _messageQueue = new();
        private Coroutine _queueCoroutine;

        private void Awake()
        {
            SetupSingleton();
        }
        
        private void OnEnable()
        {
            EventManager.I.OnGameStateChanged += OnGameStateChanged;

            _messageValue.alpha = 0;
        }
        
        private void OnDisable()
        {
            EventManager.I.OnGameStateChanged -= OnGameStateChanged;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
                Queue($"Hello World {Time.time}", 1.5f);
        }

        private void OnGameStateChanged(EGameState state)
        {
            if (state is EGameState.Loaded)
                Queue("Spawning Bricks..", 1.5f);
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

        public static void Queue(string message, float duration)
        {
            I.QueueMessage(message, duration);
        }
        
        /// <summary>
        /// Queue a message to be displayed.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="duration"></param>
        public void QueueMessage(string message, float duration)
        {
            if (_messageQueue.ContainsKey(message))
                return;

            if (duration < 0.75f)
                duration = 0.75f;
            
            _messageQueue.Add(message, duration);

            if (_queueCoroutine == null)
                _queueCoroutine = StartCoroutine(ShowMessages());
        }

        /// <summary>
        /// Shows the messages from the queue.
        /// </summary>
        /// <returns></returns>
        private IEnumerator ShowMessages()
        {
            while (_messageQueue.Count > 0)
            {
                var next = _messageQueue.FirstOrDefault();
                
                _messageValue.SetText(next.Key);
                _messageValue.DOKill(true);
                _messageValue.transform.localScale = Vector3.one * 2f;
                _messageValue.DOFade(1, _animationDuration);
                _messageValue.transform.DOScale(Vector3.one, _animationDuration)
                    .SetEase(Ease.OutCirc);

                yield return new WaitForSeconds(next.Value - _animationDuration);
                
                _messageValue.DOFade(0, _animationDuration);
                _messageValue.transform.DOScale(Vector3.zero, _animationDuration)
                    .SetEase(Ease.InCirc);

                _messageQueue.Remove(next.Key);
                
                yield return new WaitForSeconds(_animationDuration);
            }
        }
    }
}