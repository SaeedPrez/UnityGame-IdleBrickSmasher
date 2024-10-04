using DG.Tweening;
using Prez.Core;
using Prez.Utilities;
using TMPro;
using UnityEngine;

namespace Prez
{
    public class Brick : MonoBehaviour
    {
        [SerializeField] private TMP_Text _healthUi;
        [SerializeField] private SpriteRenderer _image;
        
        public Color Color { get; private set; }
        public Vector2Int GridPosition { get; private set; }
        
        private int _maxHealth = 1;
        private int _currentHealth;

        private void OnEnable()
        {
            _currentHealth = _maxHealth;
            UpdateHealthUi();
            Color = _image.color;
        }

        public void SetMaxHealth(int health)
        {
            _maxHealth = health;
            _currentHealth = _maxHealth;
        }

        public void SetPosition(Vector2Int gridPosition, Vector2 position)
        {
            GridPosition = gridPosition;
            transform.localPosition = position;
        }

        public void MoveDown(Vector2 position)
        {
            transform.DOLocalMoveY(position.y, 0.2f)
                .SetEase(Ease.OutCirc);
            
            GridPosition += Vector2Int.up;
        }
        
        public void TakeDamage(int damage)
        {
            _currentHealth -= damage;

            if (_currentHealth <= 0)
                Destroyed();
            
            UpdateHealthUi();
        }

        private void UpdateHealthUi()
        {
            _healthUi.SetText(Helper.GetFormattedNumber(_currentHealth));
        }

        private void Destroyed()
        {
            EventManager.I.TriggerBrickDestroyed(this);
            gameObject.SetActive(false);
        }
    }
}