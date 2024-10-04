using System.Collections;
using UnityEngine;

namespace Prez
{
    public class Ball : MonoBehaviour
    {
        private Rigidbody2D _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.CompareTag("Brick"))
                return;
            
            var brick = other.gameObject.GetComponent<Brick>();
            brick.TakeDamage(1);
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
