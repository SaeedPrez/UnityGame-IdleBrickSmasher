using System.Collections;
using Prez.Core;
using UnityEngine;

namespace Prez
{
    public class BallSpawner : MonoBehaviour
    {
        [SerializeField] private Ball _ballPrefab;
        [SerializeField] private Transform _ballContainer;
        [SerializeField] private Transform _ballAimLine;
        [SerializeField] private float _ballAimAngle;
        [SerializeField] private Transform _ballAimPoint;
        [SerializeField] private float _ballSpeed;

        private Ball _ball;
        private Coroutine _ballAimCoroutine;
        private int _ballAimDirection = 1;

        private void OnEnable()
        {
            EventManager.I.OnPlayerInputReleaseBall += OnPlayerInputReleaseBall;
        }

        private void OnDisable()
        {
            EventManager.I.OnPlayerInputReleaseBall -= OnPlayerInputReleaseBall;
        }

        private void OnPlayerInputReleaseBall()
        {
            if (_ball)
                FireBall();
            else
                SpawnBall();
        }

        private void SpawnBall()
        {
            _ball = Instantiate(_ballPrefab, transform, false);
            _ball.transform.localPosition = Vector3.zero;
            _ball.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            
            StartCoroutine(AimBall());
        }

        private void FireBall()
        {
            var direction = _ballAimPoint.position - transform.position;
            _ball.transform.parent = _ballContainer;
            _ball.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            _ball.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(direction.x, direction.y).normalized * _ballSpeed;
            _ball = null;
        }

        private IEnumerator AimBall()
        {
            _ballAimLine.gameObject.SetActive(true);
            _ballAimLine.localRotation = Quaternion.identity;
            
            while (_ball)
            {
                var angle = _ballAimLine.localEulerAngles.z > 180
                    ? 360 - _ballAimLine.localEulerAngles.z
                    : _ballAimLine.localEulerAngles.z;

                if (angle >= _ballAimAngle)
                    _ballAimDirection = _ballAimDirection * -1;
                
                _ballAimLine.Rotate(Vector3.forward, _ballAimDirection, Space.Self);
                
                yield return null;
            }
            
            _ballAimLine.gameObject.SetActive(false);
        }
    }
}