using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Prez
{
    public class CombatText : MonoBehaviour
    {
        [SerializeField] private TMP_Text _valueUi;
        [SerializeField] private float _duration;
        [SerializeField] private float _movementY;
        [SerializeField] private Ease _ease;
        [SerializeField] private Color _startColor;

        private void OnEnable()
        {
            transform.DOMoveY(_movementY, 1f)
                .SetEase(_ease)
                .SetRelative();

            _valueUi.DOFade(0f, 1f);
        }

        private void OnDisable()
        {
            transform.DOKill();
            _valueUi.color = _startColor;
        }

        public void SetText(string text)
        {
            _valueUi.SetText(text);
        }
    }
}