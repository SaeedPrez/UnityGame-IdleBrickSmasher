using Prez.Utilities;
using TMPro;
using UnityEngine;

namespace Prez
{
    public class Brick : MonoBehaviour
    {
        [SerializeField] private TMP_Text _healthUi;
        [SerializeField] private int _maxHealth;
        
        private int _currentHealth;

        private void OnEnable()
        {
            _currentHealth = _maxHealth;
            UpdateHealthUi();
        }

        public void TakeDamage(int damage)
        {
            _currentHealth -= damage;

            if (_currentHealth <= 0)
                Destroy(gameObject);
            
            UpdateHealthUi();
        }

        private void UpdateHealthUi()
        {
            _healthUi.SetText(Helper.GetFormattedNumber(_currentHealth));
        }
    }
}