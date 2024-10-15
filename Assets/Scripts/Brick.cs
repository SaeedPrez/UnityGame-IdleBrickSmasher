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
    [SerializeField] private SpriteRenderer _image;
        
    public bool IsActive { get; private set; }
    public Color Color { get; private set; }
    public Vector2Int GridPosition { get; private set; }
    public double SpawnedRowNumber { get; private set; }

    private BoxCollider2D _collider;
    private double _maxHealth;
    private double _currentHealth;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
    }

    private void OnEnable()
    {
        Color = _image.color;
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
        _maxHealth = health;
        _currentHealth = _maxHealth;
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
        _image.color = Color;
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

        EventManager.I.TriggerBrickDamaged(this, ball, damage);
        
        if (_currentHealth <= 0.01d)
        {
            Destroyed();
            return;
        }

        _image.DOKill(true);
        _image.DOFade(0.25f, 0.075f)
            .OnComplete(() => _image.DOFade(1f, 0.075f));
    }
    
    /// <summary>
    /// Updates the health Ui with current health.
    /// </summary>
    private void UpdateHealthUi()
    {
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
        _image.transform.DOKill(true);
            
        EventManager.I.TriggerBrickDestroyed(this, _maxHealth);
    }
}