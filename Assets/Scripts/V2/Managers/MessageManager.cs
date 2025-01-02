using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Prez.V2.Managers
{
    public class MessageManager : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _container;
        [SerializeField] private TMP_Text _messageValue;
        [SerializeField] private float _animationDuration;
        [SerializeField] private float _defaultDuration;

        private readonly Dictionary<string, float> _messageQueue = new();
        private Coroutine _queueCoroutine;

        #region Bootstrap

        private void OnEnable()
        {
            _container.alpha = 0f;
        }
        
        #endregion

        #region Messages

        /// <summary>
        ///     Queue a message to be displayed.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="duration"></param>
        public void Queue(string message, float duration = 0f)
        {
            if (_messageQueue.ContainsKey(message))
                return;

            if (duration < 0.75f)
                duration = _defaultDuration;
            
            _messageQueue.Add(message, duration);
            _queueCoroutine ??= StartCoroutine(ShowMessages());
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

                _container.DOKill(true);
                _container.DOFade(1, _animationDuration);

                _messageValue.DOKill(true);
                _messageValue.SetText(next.Key);
                _messageValue.transform.localScale = Vector3.one * 2f;
                _messageValue.transform.DOScale(Vector3.one, _animationDuration)
                    .SetEase(Ease.OutCirc);

                yield return new WaitForSeconds(next.Value - _animationDuration);

                _container.DOFade(0, _animationDuration);
                _messageValue.transform.DOScale(Vector3.zero, _animationDuration)
                    .SetEase(Ease.InCirc);

                _messageQueue.Remove(next.Key);

                yield return new WaitForSeconds(_animationDuration);

                _queueCoroutine = null;
            }
        }

        #endregion
    }
}