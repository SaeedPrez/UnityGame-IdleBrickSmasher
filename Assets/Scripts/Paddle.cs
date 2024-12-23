using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Prez.Core;
using Prez.Data;
using Prez.Enums;
using Prez.Utilities;
using UnityEngine;

namespace Prez
{
    [SelectionBase]
    public class Paddle : MonoBehaviour
    {
        [SerializeField] private float _movementLimitFromX;
        [SerializeField] private float _movementLimitToX;
        [SerializeField] private float _velocityMultiplierX;
        [SerializeField] private SpriteRenderer _borderImage;
        [SerializeField] private Transform _idleIndicator;
        [SerializeField] private Transform _fireIndicator;
        [SerializeField] [Range(0f, 1f)] private float _idleAlpha;
        [SerializeField] private Color _borderHitColor;
        [SerializeField] private ObjectPool _bulletPool;
        [SerializeField] private Transform _weapon;
        [SerializeField] private Transform _weaponFirePoint;
        [SerializeField] private float _weaponMovementRation;

        private Rigidbody2D _rigidbody;
        private Color _borderStartColor;
        private CapsuleCollider2D _collider;
        private bool _isPlayerActive;
        private Vector2 _playerInput;
        private Coroutine _idleCoroutine;
        private Coroutine _fireCoroutine;
        private bool _isFiring;
        private Dictionary<float, double> _damageHistory = new();

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _collider = GetComponent<CapsuleCollider2D>();
            _borderStartColor = _borderImage.color;
        }
        
        private void FixedUpdate()
        {
            MovePlayer();
        }

        private void OnEnable()
        {
            EventManager.I.OnGameStateChanged += OnGameStateChanged;
            EventManager.I.OnPlayerInputMove += OnPlayerInputMove;
        }

        private void OnDisable()
        {
            EventManager.I.OnGameStateChanged -= OnGameStateChanged;
            EventManager.I.OnPlayerInputMove -= OnPlayerInputMove;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.CompareTag(Constants.Ball))
                return;

            var collisionPoint = transform.InverseTransformPoint(other.GetContact(0).point);
            var ball = other.gameObject.GetComponent<Ball>();
            ball.ChangeVelocityX(collisionPoint.x * _velocityMultiplierX);
            _borderImage.DOColor(_borderHitColor, 0.05f);
            _borderImage.DOColor(_borderStartColor, 0.05f).SetDelay(0.051f);
        }

        private void OnGameStateChanged(EGameState state)
        {
            if (state is EGameState.Loaded)
                SetPlayerActive();
        }

        private void OnPlayerInputMove(Vector2 input)
        {
            _playerInput = new Vector2(input.x, 0);
        }

        #region Movement

        /// <summary>
        /// Moves the player.
        /// </summary>
        private void MovePlayer()
        {
            if (_playerInput.x == 0f)
                return;

            var x = _rigidbody.position.x + _playerInput.x * (GameManager.Data.GetPaddleSpeed() * Time.fixedDeltaTime);
            x = Mathf.Clamp(x, _movementLimitFromX, _movementLimitToX);

            _rigidbody.MovePosition(new Vector2(x, transform.position.y));

            SetPlayerActive();
            MovePlayerWeapon();
        }

        private void MovePlayerWeapon()
        {
            _weapon.localPosition = new Vector3(transform.position.x / _weaponMovementRation, _weapon.localPosition.y, 0);
        }

        #endregion

        #region Active / Idle

        /// <summary>
        ///     Sets the player to active state.
        /// </summary>
        private void SetPlayerActive()
        {
            if (_idleCoroutine != null)
                StopCoroutine(_idleCoroutine);

            _idleCoroutine = StartCoroutine(SetPlayerIdle());

            if (!_isFiring)
                _fireCoroutine = StartCoroutine(FireBullets());
            
            if (_isPlayerActive)
                return;

            _isPlayerActive = true;
            _collider.enabled = true;

            _borderImage.sharedMaterial.SetFloat(Constants.Opacity, 1f);

        }

        /// <summary>
        /// Sets the player to idle state.
        /// </summary>
        /// <returns></returns>
        private IEnumerator SetPlayerIdle()
        {
            var idleCooldown = GameManager.Data.GetPaddleIdleCooldown();
            
            _idleIndicator.DOKill();
            _idleIndicator.DOScaleX(0f, idleCooldown)
                .From(1f)
                .SetEase(Ease.Linear);

            yield return new WaitForSeconds(idleCooldown);

            _isPlayerActive = false;
            _collider.enabled = false;

            StopCoroutine(_fireCoroutine);
            _isFiring = false;

            _borderImage.sharedMaterial.SetFloat(Constants.Opacity, _idleAlpha);
        }

        #endregion

        #region Fire

        /// <summary>
        /// File bullets coroutine.
        /// </summary>
        /// <returns></returns>
        private IEnumerator FireBullets()
        {
            _isFiring = true;
            
            while (_isFiring)
            {
                var fireCooldown = 1f / GameManager.Data.GetPaddleBulletFireRate();
                
                _fireIndicator.DOKill();
                _fireIndicator.DOScaleY(10.3f, fireCooldown)
                    .From(0)
                    .SetEase(Ease.Linear);

                yield return new WaitForSeconds(fireCooldown);
                
                var bulletRight = _bulletPool.GetPooledObject();
                bulletRight.transform.position = _weaponFirePoint.position;
                bulletRight.gameObject.SetActive(true);
            }
        }

        #endregion
    }
}