using System;
using Prez.Menus;
using UnityEngine;
using UnityEngine.UI;

namespace Prez.Core
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField] private MenuBase _settings;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private MenuBase _balls;
        [SerializeField] private Button _ballsButton;
        [SerializeField] private MenuBase _player;
        [SerializeField] private Button _playerButton;
        [SerializeField] private MenuBase _level;
        [SerializeField] private Button _levelButton;

        private void OnEnable()
        {
            _ballsButton.onClick.AddListener(OnBallsButtonPressed);
        }

        private void OnDisable()
        {
            _ballsButton.onClick.RemoveListener(OnBallsButtonPressed);

        }

        private void Start()
        {
            _balls.gameObject.SetActive(true);
            _balls.Hide(true);
        }

        private void OnBallsButtonPressed()
        {
            if (_balls.gameObject.activeInHierarchy)
                _balls.Hide();
            else
                _balls.Show();
        }
    }
}