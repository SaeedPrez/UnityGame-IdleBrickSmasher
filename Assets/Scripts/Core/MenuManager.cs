using Menus;
using UnityEngine;
using UnityEngine.UI;

namespace Core
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
        [SerializeField] private MenuBase _more;
        [SerializeField] private Button _moreButton;

        private void OnEnable()
        {
            _ballsButton.onClick.AddListener(OnBallsButtonPressed);
            _playerButton.onClick.AddListener(OnPlayerButtonPressed);
            _levelButton.onClick.AddListener(OnLevelButtonPressed);
            _moreButton.onClick.AddListener(OnMoreButtonPressed);
            _settingsButton.onClick.AddListener(OnSettingsButtonPressed);
        }

        private void OnDisable()
        {
            _ballsButton.onClick.RemoveListener(OnBallsButtonPressed);
            _playerButton.onClick.RemoveListener(OnPlayerButtonPressed);
            _levelButton.onClick.RemoveListener(OnLevelButtonPressed);
            _moreButton.onClick.RemoveListener(OnMoreButtonPressed);
            _settingsButton.onClick.RemoveListener(OnSettingsButtonPressed);
        }

        private void Start()
        {
            _balls.gameObject.SetActive(true);
            _balls.Hide(true);

            _player.gameObject.SetActive(true);
            _player.Hide(true);
            
            _level.gameObject.SetActive(true);
            _level.Hide(true);
            
            _more.gameObject.SetActive(true);
            _more.Hide(true);
            
            _settings.gameObject.SetActive(true);
            _settings.Hide(true);
        }

        private void OnBallsButtonPressed()
        {
            if (_balls.gameObject.activeInHierarchy)
                _balls.Hide();
            else
            {
                CloseAllMenus();
                _balls.Show();
            }
        }
        
        private void OnPlayerButtonPressed()
        {
            if (_player.gameObject.activeInHierarchy)
                _player.Hide();
            else
            {
                CloseAllMenus();
                _player.Show();
            }
        }
        
        private void OnLevelButtonPressed()
        {
            if (_level.gameObject.activeInHierarchy)
                _level.Hide();
            else
            {
                CloseAllMenus();
                _level.Show();
            }
        }

        private void OnMoreButtonPressed()
        {
            if (_more.gameObject.activeInHierarchy)
                _more.Hide();
            else
            {
                CloseAllMenus();
                _more.Show();
            }
        }

        private void OnSettingsButtonPressed()
        {
            if (_settings.gameObject.activeInHierarchy)
                _settings.Hide();
            else
                _settings.Show();
        }
        
        private void CloseAllMenus()
        {
            if (_balls.gameObject.activeInHierarchy)
                _balls.Hide();
            
            if (_player.gameObject.activeInHierarchy)
                _player.Hide();
            
            if (_level.gameObject.activeInHierarchy)
                _level.Hide();
            
            if (_more.gameObject.activeInHierarchy)
                _more.Hide();
        }
    }
}