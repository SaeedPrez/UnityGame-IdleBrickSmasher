using System;
using Core;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Utilities;

[SelectionBase]
public class Brick : MonoBehaviour
{
    [SerializeField] private TMP_Text _healthUi;
    [SerializeField] private Transform _fillContainer;
    [SerializeField] private SpriteRenderer _fillImage;
    [SerializeField] private SpriteRenderer _borderImage;
        
    public double MaxHealth { get; private set; }
    public bool IsActive { get; private set; }
    public Color Color { get; private set; }
    public Vector2Int GridPosition { get; private set; }
    public double SpawnedRowNumber { get; private set; }

    private BoxCollider2D _collider;
    private double _currentHealth;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
    }

    private void OnEnable()
    {
        Color = _fillImage.color;
        IsActive = true;
        _collider.enabled = true;
        UpdateHealthUi();
    }
        
    /// <summary>
    /// Sets max health.
    /// </summary>
    /// <param name="health"></param>
    public void SetMaxHealth(double health)
    {
        MaxHealth = health;
        _currentHealth = MaxHealth;
    }

    /// <summary>
    /// Sets positions.
    /// </summary>
    /// <param name="gridPosition"></param>
    /// <param name="position"></param>
    public void SetPosition(Vector2Int gridPosition, Vector2 position)
    {
        GridPosition = gridPosition;
        transform.localPosition = position;
    }

    /// <summary>
    /// Set the row number where the brick belongs.
    /// </summary>
    /// <param name="row"></param>
    public void SetSpawnedRowNumber(double row)
    {
        SpawnedRowNumber = row;
    }

    /// <summary>
    /// Sets the brick color.
    /// </summary>
    /// <param name="color"></param>
    public void SetColor(Color color)
    {
        Color = color;
        _fillImage.color = Color;
    }
        
    /// <summary>
    /// Moves the brick down a row.
    /// </summary>
    /// <param name="position"></param>
    public void MoveDown(Vector2 position)
    {
        transform.DOKill(true);
        transform.DOLocalMoveY(position.y, 0.2f)
            .SetEase(Ease.OutCirc);
            
        GridPosition += Vector2Int.up;
    }

    /// <summary>
    /// Takes damage.
    /// </summary>
    /// <param name="ball"></param>
    /// <param name="damage"></param>
    public void TakeDamage(Ball ball, double damage)
    {
        if (damage > _currentHealth)
            damage = _currentHealth;
        
        _currentHealth -= Math.Round(damage, 2);
        UpdateHealthUi();

        if (_currentHealth <= 0.1d)
        {
            Destroyed();
            EventManager.I.TriggerBrickDamaged(this, ball, damage, true);
        }
        else
        {
            _fillImage.DOKill(true);
            _fillImage.DOFade(0.25f, 0.075f)
                .OnComplete(() => _fillImage.DOFade(1f, 0.075f));
            EventManager.I.TriggerBrickDamaged(this, ball, damage, false);
        }
    }
    
    /// <summary>
    /// Updates the health Ui with current health.
    /// </summary>
    private void UpdateHealthUi()
    {
        if (_currentHealth <= 0 || MaxHealth <= 0)
        {
            _fillContainer.localScale = Vector3.one;
            return;
        }
        
        _fillContainer.DOKill();
        _fillContainer.DOScaleX((float)(_currentHealth / MaxHealth), 0.1f);
        
        _healthUi.SetText(Helper.GetNumberAsString(_currentHealth));
    }

    /// <summary>
    /// Destroys the brick.
    /// </summary>
    private void Destroyed()
    {
        if (!IsActive)
            return;
            
        IsActive = false;
        _collider.enabled = false;
        transform.DOKill(true);
        _fillImage.transform.DOKill(true);
    }
}