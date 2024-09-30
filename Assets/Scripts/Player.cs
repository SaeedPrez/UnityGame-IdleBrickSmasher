using Prez.Core;
using UnityEngine;

namespace Prez
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private float _speed;
        [SerializeField] private float _minX;
        [SerializeField] private float _maxX;
        
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
            var x = _rigidbody.position.x + _playerInput.x * (_speed * Time.fixedDeltaTime);
            x = Mathf.Clamp(x, _minX, _maxX);
            
            _rigidbody.MovePosition(new Vector2(x, transform.position.y));
        }
    }
}