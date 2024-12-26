using System;
using DG.Tweening;
using Prez.Core;
using Prez.Data;
using Prez.Utilities;
using TMPro;
using UnityEngine;

namespace Prez
{
    [SelectionBase]
    public class Brick : MonoBehaviour
    {
        [SerializeField] private TMP_Text _healthUi;
        [SerializeField] private Transform _fillContainer;
        [SerializeField] private SpriteRenderer _borderImage;
        [SerializeField] private Color _borderColor;
        [SerializeField] private Color _specialColor;
        [SerializeField] private Color _damageColor;

        public double MaxHealth { get; private set; }
        public bool IsActive { get; private set; }
        public Color Color { get; private set; }
        public Vector2Int GridPosition { get; private set; }
        public double SpawnedRowNumber { get; private set; }
        public bool IsSpecial { get; private set; }

        private BoxCollider2D _collider;
        private double _currentHealth;
        
        private void Awake()
        {
            _collider = GetComponent<BoxCollider2D>();
            Color = _borderColor;
        }

        private void OnEnable()
        {
            IsActive = true;
            _collider.enabled = true;
            IsSpecial = false;
            _borderImage.color = _borderColor;
            UpdateHealthUi();
        }
        
        /// <summary>
        ///     Sets max health.
        /// </summary>
        /// <param name="health"></param>
        public void SetMaxHealth(double health)
        {
            MaxHealth = health;
            _currentHealth = MaxHealth;
        }

        /// <summary>
        ///     Sets positions.
        /// </summary>
        /// <param name="gridPosition"></param>
        /// <param name="position"></param>
        public void SetPosition(Vector2Int gridPosition, Vector2 position)
        {
            GridPosition = gridPosition;
            transform.localPosition = position;
        }

        /// <summary>
        ///     Set the row number where the brick belongs.
        /// </summary>
        /// <param name="row"></param>
        public void SetSpawnedRowNumber(double row)
        {
            SpawnedRowNumber = row;
        }

        /// <summary>
        ///     Sets the brick color.
        /// </summary>
        /// <param name="color"></param>
        private void SetColor(Color color)
        {
            Color = color;
            _borderImage.color = Color;
        }

        /// <summary>
        /// Sets the brick as special;
        /// </summary>
        public void SetSpecial()
        {
            IsSpecial = true;
            SetColor(_specialColor);
        }

        /// <summary>
        ///     Moves the brick down a row.
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
        ///     Takes damage.
        /// </summary>
        /// <param name="data"></param>
        public void TakeDamage(DamageData data)
        {
            var damage = data.Damage > _currentHealth
                ? _currentHealth
                : data.Damage;

            _currentHealth -= System.Math.Round(damage, 2);
            UpdateHealthUi();

            if (_currentHealth <= 0.1d)
            {
                data.BrickDestroyed = true;
                Destroyed();
            }
            else
            {
                _borderImage.DOKill();
                _borderImage.color = _damageColor;
                _borderImage.DOColor(Color, 0.1f)
                    .SetEase(Ease.InOutCirc);
            }

            EventManager.I.TriggerBrickDamaged(data);
        }

        /// <summary>
        ///     Updates the health Ui with current health.
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
        ///     Destroys the brick.
        /// </summary>
        private void Destroyed()
        {
            if (!IsActive)
                return;

            IsActive = false;
            _collider.enabled = false;
            transform.DOKill(true);
            _fillContainer.transform.DOKill(true);
            _borderImage.DOKill(true);
            Color = _borderColor;
        }
    }
}