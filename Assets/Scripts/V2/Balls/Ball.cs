using System.Collections;
using Prez.V2.Data;
using Prez.V2.Enums;
using Prez.V2.Managers;
using Prez.V2.Obstacles;
using UnityEngine;

namespace Prez.V2.Balls
{
    public class Ball : MonoBehaviour
    {
        [SerializeField] private TrailRenderer _trail;
        [SerializeField] private SpriteRenderer _borderSprite;
        [SerializeField] private Color _activeColor;
        
        [SerializeField] private float _tempSpeed = 5f;
        [SerializeField] private float _tempDamage = 1f;
        
        private EventManager _event;
        private GameManager _game;
        private Rigidbody2D _rigidbody;
        private Color _normalColor;
        private int _activeHits;
        private float _activeHitBoost;

        #region Bootstrap

        private void Awake()
        {
            _event = RefManager.Event;
            _game = RefManager.Game;
            _rigidbody = GetComponent<Rigidbody2D>();
            _normalColor = _borderSprite.color;
        }

        private void OnEnable()
        {
            _event.OnGameStateChanged += OnGameStateChanged;
        }

        private void OnDisable()
        {
            _event.OnGameStateChanged -= OnGameStateChanged;
        }

        private void Start()
        {
            _rigidbody.linearVelocity = new Vector2(Random.Range(-0.6f, 0.6f), Random.Range(0.5f, 1f)).normalized * _tempSpeed;
        }

        #endregion
        
        #region Observers

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag(Constants.Brick))
                CollidedWithBrick(other);
            else if (other.gameObject.CompareTag(Constants.Wall))
                CollidedWithWall(other);
            else if (other.gameObject.CompareTag(Constants.Paddle))
                CollidedWithPaddle();
            
            CheckSpeed();
        }
        
        private void OnGameStateChanged(EGameState newState, EGameState oldState)
        {
            _rigidbody.simulated = newState is EGameState.Playing;
        }

        #endregion

        #region Collisions

        /// <summary>
        /// Handles collision with paddle.
        /// </summary>
        private void CollidedWithPaddle()
        {
            EnableActiveHits();
        }
        
        /// <summary>
        /// Handles collision with bricks.
        /// </summary>
        /// <param name="other"></param>
        private void CollidedWithBrick(Collision2D other)
        {
            var brick = other.gameObject.GetComponent<Brick>();
            _event.TriggerBallCollidedWithBrick(this, brick);

            var data = new DamageData
            {
                Ball = this,
                Brick = brick,
                Damage = _tempDamage,
                CollisionPoint =  other.contacts[0].point,
                CollisionNormal =  other.contacts[0].normal.normalized,
                ActiveHit = UseActiveHit(),
                ActiveHitBoost = _activeHitBoost,
            };

            brick.TakeDamage(data);
        }

        /// <summary>
        /// Handles collision with walls.
        /// </summary>
        /// <param name="other"></param>
        private void CollidedWithWall(Collision2D other)
        {
            var obstacleSide = other.gameObject.GetComponent<ObstacleSide>();

            if (obstacleSide.Side is CollisionSide.Bottom)
                DisableActiveHits();
        }
        
        #endregion

        #region Speed

        private void CheckSpeed()
        {
            StartCoroutine(CheckSpeedAfterDelay());
        }

        private IEnumerator CheckSpeedAfterDelay()
        {
            yield return null;
            _rigidbody.linearVelocity = _rigidbody.linearVelocity.normalized * _tempSpeed;
        }

        #endregion

        #region Active Hits

        /// <summary>
        /// Enables active hits..
        /// </summary>
        private void EnableActiveHits()
        {
            _activeHits = _game.Data.PaddleData.GetActiveHits();
            _activeHitBoost = _game.Data.PaddleData.GetActiveBoost();
            _trail.emitting = true;
            _borderSprite.color = _activeColor;
        }

        /// <summary>
        /// Uses active hits if available.
        /// </summary>
        /// <returns></returns>
        private bool UseActiveHit()
        {
            if (_activeHits <= 0)
                return false;

            _activeHits--;

            if (_activeHits <= 0)
                DisableActiveHits();
            
            return true;
        }

        /// <summary>
        /// Disables the active hits.
        /// </summary>
        /// <returns></returns>
        private void DisableActiveHits()
        {
            _activeHits = 0;
            _activeHitBoost = 0;
            _trail.emitting = false;
            _borderSprite.color = _normalColor;
        }

        #endregion
    }
}
