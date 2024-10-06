using Prez.Data;
using TMPro;
using UnityEngine;

namespace Prez.Core
{
    public class CoinsManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text _coinsValueUi;
        
        private EventManager _event;
        private GameData _data;

        private void Awake()
        {
            _event = EventManager.I;
            _data = GameManager.I.Data;
        }

        private void OnEnable()
        {
            _event.OnBrickDestroyed += OnBrickDestroyed;
        }
        
        private void OnDisable()
        {
            _event.OnBrickDestroyed -= OnBrickDestroyed;
        }
        
        private void OnBrickDestroyed(Brick brick, long maxHealth)
        {
            _data.Coins.Add(maxHealth);
            UpdateCoinsValueUi();
        }

        /// <summary>
        /// Updates coins value Ui.
        /// </summary>
        private void UpdateCoinsValueUi()
        {
            _coinsValueUi.text = _data.Coins.AsString;
        }
    }
}