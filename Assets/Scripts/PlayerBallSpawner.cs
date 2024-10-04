using System.Collections;
using Prez.Core;
using UnityEngine;

namespace Prez
{
    public class PlayerBallSpawner : MonoBehaviour
    {
        [SerializeField] private Ball _ballPrefab;
        [SerializeField] private Transform _ballSpawner;
        [SerializeField] private Transform _ballContainer;
        [SerializeField] private float _ballSpeed;
        [SerializeField] private Transform _aimLine;
        [SerializeField] private float _aimMaxAngle;
        [SerializeField] private Transform _aimPoint;

        private Ball _ball;
        private Rigidbody2D _ballRigidBody;
        private Coroutine _ballAimCoroutine;
        private int _ballAimDirection = 1;

        private void OnEnable()
        {
            EventManager.I.OnPlayerInputBallAction1 += OnPlayerInputBallAction1;
            
            _aimLine.gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            EventManager.I.OnPlayerInputBallAction1 -= OnPlayerInputBallAction1;
        }

        private void OnPlayerInputBallAction1()
        {
            if (_ball)
                FireBall();
            else
                SpawnBall();
        }

        private void SpawnBall()
        {
            _ball = Instantiate(_ballPrefab, _ballSpawner, false);
            _ball.transform.localPosition = Vector3.zero;
            _ballRigidBody = _ball.GetComponent<Rigidbody2D>();
            _ballRigidBody.bodyType = RigidbodyType2D.Kinematic;
            
            StartCoroutine(AimBall());
        }

        private void FireBall()
        {
            var direction = _aimPoint.position - transform.position;
            _ball.transform.parent = _ballContainer;
            _ballRigidBody.bodyType = RigidbodyType2D.Dynamic;
            _ballRigidBody.linearVelocity = new Vector2(direction.x, direction.y).normalized * _ballSpeed;
            _ballRigidBody = null;
            _ball = null;
        }

        private IEnumerator AimBall()
        {
            _aimLine.gameObject.SetActive(true);
            _aimLine.localRotation = Quaternion.identity;
            
            while (_ball)
            {
                var angle = _aimLine.localEulerAngles.z > 180
                    ? 360 - _aimLine.localEulerAngles.z
                    : _aimLine.localEulerAngles.z;

                if (angle >= _aimMaxAngle)
                    _ballAimDirection = _ballAimDirection * -1;
                
                _aimLine.Rotate(Vector3.forward, _ballAimDirection, Space.Self);
                
                yield return null;
            }
            
            _aimLine.gameObject.SetActive(false);
        }
    }
}