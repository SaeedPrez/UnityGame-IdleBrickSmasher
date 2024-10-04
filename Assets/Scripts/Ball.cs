using System.Collections;
using Prez.Core;
using Prez.Data;
using UnityEngine;

namespace Prez
{
    [SelectionBase]
    public class Ball : MonoBehaviour
    {
        private GameData _date;
        private Rigidbody2D _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _date = GameManager.I.Data;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.CompareTag("Brick"))
                return;
            
            var brick = other.gameObject.GetComponent<Brick>();
            brick.TakeDamage(_date.BallDamageBase);
        }

        public void ChangeVelocityX(float value)
        {
            StartCoroutine(ChangeVelocityXAfterDelay(value));
        }

        private IEnumerator ChangeVelocityXAfterDelay(float value, float delay = 0.01f)
        {
            yield return new WaitForSeconds(delay);

            var speed = _rigidbody.linearVelocity.magnitude;
            _rigidbody.linearVelocity = new Vector2(value, _rigidbody.linearVelocity.y).normalized * 5f;
        }
    }
}
