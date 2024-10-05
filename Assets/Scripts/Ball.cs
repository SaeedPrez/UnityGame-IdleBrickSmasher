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
        
        private GameData _date;
        private Rigidbody2D _rigidbody;
        private bool _isPlayerBoostActive;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _date = GameManager.I.Data;
            _trail.emitting = false;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                EnableActivePlayBoost();
                return;
            }

            if (other.gameObject.CompareTag("WallBottom"))
            {
                DisableActivePlayBoost();
                return;
            }

            if (!other.gameObject.CompareTag("Brick"))
                return;

            var brick = other.gameObject.GetComponent<Brick>();
            brick.TakeDamage(_date.BallDamageBase * (_isPlayerBoostActive ? 2 : 1));
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
            _rigidbody.linearVelocity = new Vector2(value, _rigidbody.linearVelocity.y).normalized * 5f;
        }

        public void EnableActivePlayBoost()
        {
            _isPlayerBoostActive = true;
            _trail.emitting = true;
        }

        public void DisableActivePlayBoost()
        {
            _isPlayerBoostActive = false;
            _trail.emitting = false;
        }
    }
}
