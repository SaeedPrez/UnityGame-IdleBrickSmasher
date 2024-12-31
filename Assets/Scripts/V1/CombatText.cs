using DG.Tweening;
using Prez.V1.Data;
using Prez.V1.Utilities;
using TMPro;
using UnityEngine;

namespace Prez.V1
{
    public class CombatText : MonoBehaviour
    {
        [SerializeField] private TMP_Text _valueUi;
        [SerializeField] private float _duration;
        [SerializeField] private float _movementY;
        [SerializeField] private Ease _ease;
        [SerializeField] private Color _startColor;
        [SerializeField] private Color _activeHitColor;
        [SerializeField] private float _criticalHitScale;
        

        private void OnEnable()
        {
            transform.DOMoveY(_movementY, _duration)
                .SetEase(_ease)
                .SetRelative();

            _valueUi.DOFade(0f, _duration);
        }

        private void OnDisable()
        {
            transform.DOKill();
        }

        public void SetData(DamageData data)
        {
            var damage = Helper.GetNumberAsString(data.Damage);
            _valueUi.SetText(damage);

            _valueUi.color = data.ActiveHit
                ? _activeHitColor
                : _startColor;

            transform.localScale = data.CriticalHit
                ? Vector3.one * _criticalHitScale 
                : Vector3.one;

            if (data.CriticalHit)
            {
                transform.DOScale(Vector3.one, _duration / 3f)
                    .SetEase(_ease)
                    .SetDelay(_duration / 3f);
            }
        }
    }
}