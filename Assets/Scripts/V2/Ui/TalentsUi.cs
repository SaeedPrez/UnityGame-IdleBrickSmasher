using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Prez.V2.Ui
{
    public class TalentsUi : MonoBehaviour, IDragHandler, IScrollHandler
    {
        [SerializeField] private float _zoomFrom;
        [SerializeField] private float _zoomTo;
        [SerializeField] private float _zoomStep;
        
        private RectTransform _rectTransform;
        private Canvas _canvas;
        private float _zoom = 1f;

        #region Bootstrap

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvas = GetComponentInParent<Canvas>();
        }

        #endregion

        #region Observers

        public void OnDrag(PointerEventData eventData)
        {
            if (Input.GetMouseButton(1))
                Move(eventData.delta);
        }

        public void OnScroll(PointerEventData eventData)
        {
            Zoom(eventData.scrollDelta.y);
        }

        #endregion

        #region Talents Window

        /// <summary>
        /// Moves the talents window.
        /// </summary>
        /// <param name="delta"></param>
        private void Move(Vector2 delta)
        {
            _rectTransform.anchoredPosition += delta / _canvas.scaleFactor;
        }

        /// <summary>
        /// Zooms the talents window.
        /// </summary>
        /// <param name="delta"></param>
        private void Zoom(float delta)
        {
            var scrollDelta = delta < 0f ? -1f : 1f;
            var newZoom = Mathf.Clamp(_zoom + (scrollDelta * _zoomStep), _zoomFrom, _zoomTo);

            if (Mathf.Approximately(_zoom, newZoom))
                return;

            _zoom = newZoom;
            _rectTransform.DOKill();
            _rectTransform.DOScale(_zoom, 0.2f)
                .SetEase(Ease.OutCirc);

        }

        #endregion
    }
}