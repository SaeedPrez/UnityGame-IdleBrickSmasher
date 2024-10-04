using Prez.Core;
using UnityEngine;

namespace Prez
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private float _baseSpeed;
        [SerializeField] private float _movementLimitToX;
        [SerializeField] private float _movementLimitFromX;
        [SerializeField] private Transform _circle;
        [SerializeField] private float _velocityMultiplierX;
        
        private Rigidbody2D _rigidbody;
        private Vector2 _playerInput;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void OnEnable()
        {
            EventManager.I.OnPlayerInputMove += OnPlayerInputMove;
        }

        private void OnDisable()
        {
            EventManager.I.OnPlayerInputMove -= OnPlayerInputMove;
        }

        private void OnPlayerInputMove(Vector2 input)
        {
            _playerInput = new Vector2(input.x, 0);
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
    }
}