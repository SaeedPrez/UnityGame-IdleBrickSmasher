using Prez.V2.Data;
using Prez.V2.Utilities;
using UnityEngine;

namespace Prez.V2.Managers
{
    public class BrickEffectManager : MonoBehaviour
    {
        [SerializeField] private ObjectPool _brickHitObjectPool;
        [SerializeField] private ObjectPool _brickDestroyObjectPool;

        private EventManager _event;
        
        #region Bootstrap

        private void Awake()
        {
            _event = RefManager.Event;
        }

        private void OnEnable()
        {
            _event.OnBrickDamaged += OnBrickDamaged;
        }

        private void OnDisable()
        {
            _event.OnBrickDamaged -= OnBrickDamaged;
        }

        #endregion

        #region Observers

        private void OnBrickDamaged(DamageData data)
        {
            if (data.BrickDestroyed)
                PlayDestroyEffect(data);
            else
                PlayHitEffect(data);
        }

        #endregion

        #region Effects

        /// <summary>
        /// Plays the brick hit effect.
        /// </summary>
        /// <param name="data"></param>
        private void PlayHitEffect(DamageData data)
        {
            var effect = _brickHitObjectPool.GetPooledObject();
            effect.transform.position = data.CollisionPoint;
            SetEffectColor(effect, data.Brick.Color);
            effect.SetActive(true);
        }

        /// <summary>
        /// Plays the brick destroyed effect.
        /// </summary>
        /// <param name="data"></param>
        private void PlayDestroyEffect(DamageData data)
        {
            var effect = _brickDestroyObjectPool.GetPooledObject();
            effect.transform.position = data.Brick.transform.position;
            SetEffectColor(effect, data.Brick.Color);
            effect.SetActive(true);
        }

        /// <summary>
        /// Sets the effect color.
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="color"></param>
        private void SetEffectColor(GameObject effect, Color color)
        {
            var particle = effect.GetComponent<ParticleSystem>();
            var main = particle.main;
            main.startColor = color;
        }

        #endregion
    }
}