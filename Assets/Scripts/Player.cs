using System.Collections;
using Core;
using Data;
using DG.Tweening;
using Enums;
using UnityEngine;

[SelectionBase]
public class Player : MonoBehaviour
{
    [SerializeField] private float _movementLimitToX;
    [SerializeField] private float _movementLimitFromX;
    [SerializeField] private float _velocityMultiplierX;
    [SerializeField] private SpriteRenderer _bgImage;
    [SerializeField] private SpriteRenderer _borderImage;
    [SerializeField] private Transform _cooldownIndicator;
    [SerializeField, Range(0f, 1f)] private float _idleAlpha;
    [SerializeField] private Color _borderHitColor;
        
    private Rigidbody2D _rigidbody;
    private CapsuleCollider2D _collider;
    private Vector2 _playerInput;
    private bool _isPlayerActive;
    private float _playerIdleCooldown;
    private Color _borderStartColor;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<CapsuleCollider2D>();
        _borderStartColor = _borderImage.color;
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

    private void OnGameStateChanged(EGameState state)
    {
        if (state is EGameState.Loaded)
        {
            SetPlayerActive();
            StartCoroutine(SetPlayerIdle());
        }
    }
    
    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void OnPlayerInputMove(Vector2 input)
    {
        _playerInput = new Vector2(input.x, 0);
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

    /// <summary>
    /// Moves the player.
    /// </summary>
    private void MovePlayer()
    {
        if (_playerInput.x == 0f)
            return;
            
        var x = _rigidbody.position.x + _playerInput.x * (GameManager.Data.GetPlayerSpeed() * Time.fixedDeltaTime);
        x = Mathf.Clamp(x, _movementLimitToX, _movementLimitFromX);
            
        _rigidbody.MovePosition(new Vector2(x, transform.position.y));
        SetPlayerActive();
    }

    /// <summary>
    /// Sets the player to active state.
    /// </summary>
    private void SetPlayerActive()
    {
        _playerIdleCooldown = GameManager.Data.GetPlayerIdleCooldown();
            
        if (_isPlayerActive)
            return;
            
        _isPlayerActive = true;
        _collider.enabled = true;

        _bgImage.DOKill();
        _bgImage.DOFade(1, 0.1f);
          
        _borderImage.DOKill();
        _borderImage.DOFade(1, 0.1f);
    }

    /// <summary>
    /// Sets the player to idle state.
    /// </summary>
    /// <returns></returns>
    private IEnumerator SetPlayerIdle()
    {
        var checkDelay = 0.2f;
            
        while (true)
        {
            yield return new WaitForSeconds(checkDelay);

            if (!_isPlayerActive)
                continue;

            _playerIdleCooldown -= checkDelay;
            var percent = _playerIdleCooldown / GameManager.Data.GetPlayerIdleCooldown();

            _cooldownIndicator.DOKill();
            _cooldownIndicator.DOScaleX(percent, checkDelay)
                .SetEase(Ease.Linear);
                
            if (_playerIdleCooldown > 0f)
                continue;

            _isPlayerActive = false;
            _collider.enabled = false;

            _bgImage.DOKill();
            _bgImage.DOFade(_idleAlpha, checkDelay);
                
            _borderImage.DOKill();
            _borderImage.DOFade(_idleAlpha / 2f, checkDelay);
        }
    }
}