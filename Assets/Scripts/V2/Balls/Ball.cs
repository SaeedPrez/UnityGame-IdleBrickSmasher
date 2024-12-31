using Prez.V2.Data;
using Prez.V2.Managers;
using Prez.V2.Obstacles;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Prez.V2.Balls
{
    public class Ball : MonoBehaviour
    {
        protected Rigidbody2D _rigidbody;
        
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            _rigidbody.linearVelocity = Random.insideUnitCircle.normalized * 5f;
        }

        #region Observers

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag(Constants.Brick))
                CollidedWithBrick(other);
        }

        #endregion

        #region Collisions

        private void CollidedWithBrick(Collision2D other)
        {
            var brick = other.gameObject.GetComponent<Brick>();
            EventManager.TriggerBallCollidedWithBrick(this, brick);
        }
        
        #endregion
    }
}
