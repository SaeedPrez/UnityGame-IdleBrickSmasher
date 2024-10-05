using DG.Tweening;
using Prez.Core;
using Prez.Utilities;
using TMPro;
using UnityEngine;

namespace Prez
{
    [SelectionBase]
    public class Brick : MonoBehaviour
    {
        [SerializeField] private TMP_Text _healthUi;
        [SerializeField] private SpriteRenderer _image;
        
        public bool IsActive { get; private set; }
        public Color Color { get; private set; }
        public Vector2Int GridPosition { get; private set; }

        private EventManager _event;
        private BoxCollider2D _collider;
        private int _maxHealth = 1;
        private int _currentHealth;

        private void Awake()
        {
            _event = EventManager.I;;
            _collider = GetComponent<BoxCollider2D>();
        }

        private void OnEnable()
        {
            _currentHealth = _maxHealth;
            Color = _image.color;
            IsActive = true;
            _collider.enabled = true;
            UpdateHealthUi();
        }
        
        /// <summary>
        /// Sets max health.
        /// </summary>
        /// <param name="health"></param>
        public void SetMaxHealth(int health)
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
            transform.DOLocalMoveY(position.y, 0.2f)
                .SetEase(Ease.OutCirc);
            
            GridPosition += Vector2Int.up;
        }
        
        /// <summary>
        /// Takes damage.
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(int damage)
        {
            _currentHealth -= damage;

            if (_currentHealth <= 0)
                Destroyed();
            
            UpdateHealthUi();
        }

        /// <summary>
        /// Updates the health Ui with current health.
        /// </summary>
        private void UpdateHealthUi()
        {
            _healthUi.SetText(Helper.GetFormattedNumber(_currentHealth));
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
            transform.DOKill();
            _event.TriggerBrickDestroyed(this);
        }
    }
}