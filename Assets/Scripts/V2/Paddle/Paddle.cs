using DG.Tweening;
using Prez.V2.Data;
using Prez.V2.Enums;
using Prez.V2.Managers;
using UnityEngine;

namespace Prez.V2.Paddle
{
    public class Paddle : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _borderSprite;
        [SerializeField] private Color _hitColor;
        [SerializeField] private float _animationDuration;
        [SerializeField] private float _movementLimitFromX;
        [SerializeField] private float _movementLimitToX;

        private EventManager _event;
        private GameManager _game;
        private Rigidbody2D _rigidbody;
        private Vector2 _playerInput;
        private Color _normalColor;

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
            _event.OnPlayerInputMove += OnPlayerInputMove;
        }

        private void OnDisable()
        {
            _event.OnPlayerInputMove -= OnPlayerInputMove;
        }

        private void FixedUpdate()
        {
            Move();
        }

        #endregion

        #region Observers

        private void OnPlayerInputMove(Vector2 input)
        {
            _playerInput = new Vector2(input.x, 0);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag(Constants.Ball))
                AnimateBorderColor();
        }

        #endregion

        #region Movement

        /// <summary>
        /// Moves the player.
        /// </summary>
        private void Move()
        {
            if (_game.State != EGameState.Playing)
                return;
            
            if (_playerInput.x == 0f)
                return;

            var x = _rigidbody.position.x + _playerInput.x * (_game.Data.PaddleData.GetSpeed() * Time.fixedDeltaTime);
            x = Mathf.Clamp(x, _movementLimitFromX, _movementLimitToX);

            _rigidbody.MovePosition(new Vector2(x, transform.position.y));
        }

        #endregion

        #region Animations

        /// <summary>
        /// Animates the border color.
        /// </summary>
        private void AnimateBorderColor()
        {
            _borderSprite.color = _hitColor;
            _borderSprite.DOColor(_normalColor, _animationDuration)
                .SetEase(Ease.InCirc);
        }

        #endregion
    }
}