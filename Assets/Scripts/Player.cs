using System.Collections;
using DG.Tweening;
using Prez.Core;
using UnityEngine;

namespace Prez
{
    [SelectionBase]
    public class Player : MonoBehaviour
    {
        [SerializeField] private float _baseSpeed;
        [SerializeField] private float _movementLimitToX;
        [SerializeField] private float _movementLimitFromX;
        [SerializeField] private Transform _circle;
        [SerializeField] private float _velocityMultiplierX;
        [SerializeField] private float _playerIdleTime;
        [SerializeField] private SpriteRenderer _sprite;
        [SerializeField, Range(0f, 1f)] private float _idleAlpha;
        
        private Rigidbody2D _rigidbody;
        private CapsuleCollider2D _collider;
        private Vector2 _playerInput;
        private bool _isPlayerActive;
        private float _playerIdleAt;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _collider = GetComponent<CapsuleCollider2D>();
        }

        private void OnEnable()
        {
            EventManager.I.OnPlayerInputMove += OnPlayerInputMove;
            SetPlayerActive();
            StartCoroutine(SetPlayerIdle());
        }

        private void OnDisable()
        {
            EventManager.I.OnPlayerInputMove -= OnPlayerInputMove;
        }

        private void OnPlayerInputMove(Vector2 input)
        {
            _playerInput = new Vector2(input.x, 0);
            SetPlayerActive();
        }

        private void FixedUpdate()
        {
            var x = _rigidbody.position.x + _playerInput.x * (_baseSpeed * Time.fixedDeltaTime);
            x = Mathf.Clamp(x, _movementLimitToX, _movementLimitFromX);
            
            _rigidbody.MovePosition(new Vector2(x, transform.position.y));
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.CompareTag("Ball"))
                return;

            var collisionPoint = transform.InverseTransformPoint(other.GetContact(0).point);
            var ball = other.gameObject.GetComponent<Ball>();
            ball.ChangeVelocityX(collisionPoint.x * _velocityMultiplierX);
        }

        /// <summary>
        /// Sets the player to active state.
        /// </summary>
        private void SetPlayerActive()
        {
            _playerIdleAt = Time.time + _playerIdleTime;
            
            if (_isPlayerActive)
                return;
            
            _isPlayerActive = true;
            _collider.enabled = true;
            _sprite.DOKill();
            _sprite.DOFade(1, 0.1f);
        }

        /// <summary>
        /// Sets the player to idle state.
        /// </summary>
        /// <returns></returns>
        private IEnumerator SetPlayerIdle()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.25f);

                if (!_isPlayerActive)
                    continue;
                
                if (_playerIdleAt > Time.time)
                    continue;

                _isPlayerActive = false;
                _collider.enabled = false;
                _sprite.DOKill();
                _sprite.DOFade(_idleAlpha, 0.25f);
            }
        }
    }
}