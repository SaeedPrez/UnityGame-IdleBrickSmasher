using System.Collections;
using Core;
using Data;
using UnityEngine;

public class PlayerBallSpawner : MonoBehaviour
{
    [SerializeField] private Ball _ballPrefab;
    [SerializeField] private Transform _ballSpawner;
    [SerializeField] private Transform _ballContainer;
    [SerializeField] private Transform _aimLine;
    [SerializeField] private float _aimMaxAngle;
    [SerializeField] private Transform _aimPoint;
    [SerializeField] private float _aimSpeed;

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

    /// <summary>
    /// Spawn a new ball.
    /// </summary>
    private void SpawnBall()
    {
        _ball = Instantiate(_ballPrefab, _ballSpawner, false);
        _ball.transform.localPosition = Vector3.zero;
        _ballRigidBody = _ball.GetComponent<Rigidbody2D>();
        _ballRigidBody.bodyType = RigidbodyType2D.Kinematic;
            
        StartCoroutine(AnimateAimBall());
    }

    /// <summary>
    /// Fires ball at aimed direction.
    /// </summary>
    private void FireBall()
    {
        var direction = _aimPoint.position - transform.position;
        _ball.transform.parent = _ballContainer;
        _ballRigidBody.bodyType = RigidbodyType2D.Dynamic;
        _ballRigidBody.linearVelocity = new Vector2(direction.x, direction.y).normalized * GameManager.Data.BallSpeedBase;
        _ballRigidBody = null;
        _ball = null;
    }

    /// <summary>
    /// Animates the ball aim between angles.
    /// </summary>
    /// <returns></returns>
    private IEnumerator AnimateAimBall()
    {
        _aimLine.gameObject.SetActive(true);
        // _aimLine.localRotation = Quaternion.identity;
            
        while (_ball)
        {
            var angle = _aimLine.localEulerAngles.z > 180
                ? 360 - _aimLine.localEulerAngles.z
                : _aimLine.localEulerAngles.z;

            if (angle >= _aimMaxAngle)
                _ballAimDirection *= -1;
                
            _aimLine.Rotate(Vector3.forward, _ballAimDirection * _aimSpeed, Space.Self);
                
            yield return null;
        }
            
        _aimLine.gameObject.SetActive(false);
    }
}