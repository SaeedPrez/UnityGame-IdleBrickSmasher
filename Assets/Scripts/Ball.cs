using System.Collections;
using Prez.Core;
using Prez.Data;
using UnityEngine;

namespace Prez
{
    [SelectionBase]
    public class Ball : MonoBehaviour
    {
        [SerializeField] private TrailRenderer _trail;
        
        private GameData _gameData;
        private Rigidbody2D _rigidbody;
        private bool _isPlayerBoostActive;
        private int _activePlayBoostHits;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _gameData = GameManager.I.Data;
            _trail.emitting = false;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag(Constants.Player))
            {
                EnableActivePlayBoost();
                return;
            }

            if (other.gameObject.CompareTag(Constants.WallBottom))
            {
                DisableActivePlayBoost();
                return;
            }

            if (!other.gameObject.CompareTag(Constants.Brick))
                return;

            var brick = other.gameObject.GetComponent<Brick>();
            ApplyDamageToBrick(brick);
        }

        /// <summary>
        /// Changes ball X velocity.
        /// </summary>
        /// <param name="value"></param>
        public void ChangeVelocityX(float value)
        {
            StartCoroutine(ChangeVelocityXAfterDelay(value));
        }

        /// <summary>
        /// Changes ball X velocity after a delay.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        private IEnumerator ChangeVelocityXAfterDelay(float value, float delay = 0.01f)
        {
            yield return new WaitForSeconds(delay);

            // var speed = _rigidbody.linearVelocity.magnitude;
            _rigidbody.linearVelocity = new Vector2(value, _rigidbody.linearVelocity.y).normalized * _gameData.BallSpeedBase;
        }

        /// <summary>
        /// Activates the active play boost.
        /// </summary>
        public void EnableActivePlayBoost()
        {
            _isPlayerBoostActive = true;
            _trail.emitting = true;
            _activePlayBoostHits = _gameData.BrickActiveBoostHitsBase;
        }

        /// <summary>
        /// Disables the active player boost.
        /// </summary>
        public void DisableActivePlayBoost()
        {
            _isPlayerBoostActive = false;
            _trail.emitting = false;
            _activePlayBoostHits = 0;
        }

        /// <summary>
        /// Applies damage to brick.
        /// </summary>
        /// <param name="brick"></param>
        private void ApplyDamageToBrick(Brick brick)
        {
            var damage = _gameData.BallDamageBase;

            if (_isPlayerBoostActive)
                damage *= 2;

            brick.TakeDamage(damage);

            DecreaseActivePlayBoostHits();
        }

        /// <summary>
        /// Decreases active play boost hits.
        /// </summary>
        private void DecreaseActivePlayBoostHits()
        {
            if (_activePlayBoostHits <= 0)
                return;
            
            _activePlayBoostHits--;
                
            if (_activePlayBoostHits == 0)
                DisableActivePlayBoost();
        }
    }
}
