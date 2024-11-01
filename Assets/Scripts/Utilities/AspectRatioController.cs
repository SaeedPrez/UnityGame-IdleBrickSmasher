using System.Collections;
using UnityEngine;

namespace Prez.Utilities
{
    public class AspectRatioController : MonoBehaviour
    {
        [SerializeField] private Vector2 _targetAspectRatio;
        [SerializeField] private float _controlCooldown;

        private Camera _camera;
        private Vector2Int _screenSize;

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void Start()
        {
            StartCoroutine(ForceAspectRatio());
        }

        public IEnumerator ForceAspectRatio()
        {
            while (true)
            {
                if (_screenSize.x == Screen.width && _screenSize.y == Screen.height)
                {
                    yield return new WaitForSeconds(_controlCooldown);
                    continue;
                }

                _screenSize = new Vector2Int(Screen.width, Screen.height);
                var targetAspectRatio = _targetAspectRatio.x / _targetAspectRatio.y;
                var screenAspectRatio = _screenSize.x / (float)_screenSize.y;
                var scaleHeight = screenAspectRatio / targetAspectRatio;
                var scaleWidth = 1f / scaleHeight;

                var rect = _camera.rect;

                if (scaleHeight > 1f)
                {
                    rect.width = 1f;
                    rect.height = scaleHeight;
                    rect.x = 0;
                    rect.y = (1f - scaleHeight) / 2f;
                }
                else if (scaleHeight < 1f)
                {
                    rect.height = 1f;
                    rect.width = scaleWidth;
                    rect.y = 0;
                    rect.x = (1f - scaleWidth) / 2f;
                }

                _camera.rect = rect;
            }
        }
    }
}