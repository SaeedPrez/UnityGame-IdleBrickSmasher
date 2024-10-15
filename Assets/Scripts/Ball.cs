using System.Collections;
using Core;
using Data;
using UnityEngine;
using Random = UnityEngine.Random;

[SelectionBase]
public class Ball : MonoBehaviour
{
    [SerializeField] private TrailRenderer _trail;

    public bool IsPlayerBoostActive { get; private set; }
    public BallData Data { get; private set; }
        
    private Rigidbody2D _rigidbody;
    private int _activePlayBoostHits;
    private Coroutine _ballVelocityCoroutine;
    private float _lastHitAt;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _trail.emitting = false;
    }

    private void OnEnable()
    {
        _ballVelocityCoroutine = StartCoroutine(RespawnBallIfStucked());
        _lastHitAt = Time.time;
    }

    private void OnDisable()
    {
        StopCoroutine(_ballVelocityCoroutine);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag(Constants.Player))
        {
            EventManager.I.TriggerBallCollidedWithPlayer(this);
            _lastHitAt = Time.time;
        }
        else if (other.gameObject.CompareTag(Constants.WallBottom))
        {
            EventManager.I.TriggerBallCollidedWithBottomWall(this);
        }
        else if (other.gameObject.CompareTag(Constants.Brick))
        {
            var brick = other.gameObject.GetComponent<Brick>();
            EventManager.I.TriggerBallCollidedWithBrick(this, brick);
            _lastHitAt = Time.time;
        }
    }
        
    public void SetData(BallData data)
    {
        Data = data;
    }
        
    #region Velocity

    /// <summary>
    /// Sets a random direction velocity.
    /// </summary>
    public void SetRandomDirectionVelocity()
    {
        _rigidbody.linearVelocity = new Vector3(Random.Range(-1f, 1f), 1, 0).normalized * GameManager.Data.GetBallSpeed(this);
    }
        
    /// <summary>
    /// Changes ball X velocity.
    /// </summary>
    /// <param name="value"></param>
    public void ChangeVelocityX(float value)
    {
        StartCoroutine(ChangeVelocityXAfterDelay(value));
    }

    /// <summary>
    /// Changes ball X velocity after a delay.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="delay"></param>
    /// <returns></returns>
    private IEnumerator ChangeVelocityXAfterDelay(float value, float delay = 0.01f)
    {
        yield return new WaitForSeconds(delay);

        // var speed = _rigidbody.linearVelocity.magnitude;
        _rigidbody.linearVelocity = new Vector2(value, _rigidbody.linearVelocity.y).normalized * GameManager.Data.GetBallSpeed(this);
    }

    /// <summary>
    /// Checks and respawns the ball if stuck.
    /// </summary>
    /// <returns></returns>
    private IEnumerator RespawnBallIfStucked()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);

            var expectedVelocity = GameManager.Data.GetBallSpeed(this);
            var currentVelocity = _rigidbody.linearVelocity.magnitude;

            if (_lastHitAt < Time.time - 10f)
                EventManager.I.TriggerBallRequestRespawn(this);
            else if (currentVelocity <= expectedVelocity * 0.5f) 
                EventManager.I.TriggerBallRequestRespawn(this);
            else if (currentVelocity <= expectedVelocity * 0.8f || currentVelocity >= expectedVelocity * 1.25f)
                _rigidbody.linearVelocity = _rigidbody.linearVelocity.normalized * expectedVelocity;
        }
    }
    
    #endregion

    #region Active Player Boost

    /// <summary>
    /// Activates the active play boost.
    /// </summary>
    public void EnableActivePlayBoost()
    {
        IsPlayerBoostActive = true;
        _trail.emitting = true;
        _activePlayBoostHits = GameManager.Data.GetActivePlayHits(this);
    }

    /// <summary>
    /// Reduces active play boost hits.
    /// </summary>
    public void ReduceActivePlayBoostHits()
    {
        if (_activePlayBoostHits <= 0)
            return;
            
        _activePlayBoostHits--;
                
        if (_activePlayBoostHits == 0)
            DisableActivePlayBoost();
    }
        
    /// <summary>
    /// Disables the active player boost.
    /// </summary>
    public void DisableActivePlayBoost()
    {
        IsPlayerBoostActive = false;
        _trail.emitting = false;
        _activePlayBoostHits = 0;
    }

    #endregion
}