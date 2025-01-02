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
            _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
        }

        public void OnScroll(PointerEventData eventData)
        {
            var scrollDelta = eventData.scrollDelta.y < 0f ? -1f : 1f;
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