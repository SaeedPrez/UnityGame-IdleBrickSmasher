using System.Collections;
using Core;
using Data;
using UnityEngine;

[SelectionBase]
public class Ball : MonoBehaviour
{
    [SerializeField] private TrailRenderer _trail;

    public bool IsPlayerBoostActive { get; private set; }
    public BallData Data { get; private set; }
        
    private EventManager _event;
    private GameData _gameData;
    private Rigidbody2D _rigidbody;
    private int _activePlayBoostHits;

    private void Awake()
    {
        _event = EventManager.I;
        _rigidbody = GetComponent<Rigidbody2D>();
        _gameData = GameManager.I.Data;
        _trail.emitting = false;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag(Constants.Player))
            _event.TriggerBallCollidedWithPlayer(this);

        else if (other.gameObject.CompareTag(Constants.WallBottom))
            _event.TriggerBallCollidedWithBottomWall(this);
            
        else if (other.gameObject.CompareTag(Constants.Brick))
        {
            var brick = other.gameObject.GetComponent<Brick>();
            _event.TriggerBallCollidedWithBrick(this, brick);
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
        _rigidbody.linearVelocity = new Vector3(Random.Range(-1f, 1f), 1, 0).normalized * _gameData.GetBallSpeed(this);
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
        _rigidbody.linearVelocity = new Vector2(value, _rigidbody.linearVelocity.y).normalized * _gameData.GetBallSpeed(this);
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
        _activePlayBoostHits = _gameData.GetActivePlayHits(this);
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