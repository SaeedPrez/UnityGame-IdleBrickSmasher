using Prez.Core;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Prez.Menus
{
    public class StartMenu : MonoBehaviour
    {
        [SerializeField] private Button _playButton;

        [SerializeField] private Transform _gameDataUi;
        [SerializeField] private Button _showDataButton;
        [SerializeField] private Button _hideDataButton;
        [SerializeField] private TMP_InputField _gameDataInput;
        [SerializeField] private Button _loadGameDataButton;
        [SerializeField] private Button _saveGameDataButton;
        [SerializeField] private Button _resetGameDataButton;
        [SerializeField] private TMP_Text _versionLabel;

        private void OnEnable()
        {
            _playButton.onClick.AddListener(OnPlayButtonClicked);
            _showDataButton.onClick.AddListener(OnShowDataMenuButtonClicked);
            _hideDataButton.onClick.AddListener(OnHideDataMenuButtonClicked);
            _saveGameDataButton.onClick.AddListener(OnSaveGameDataButtonClicked);
            _loadGameDataButton.onClick.AddListener(OnLoadGameDataButton);
            _resetGameDataButton.onClick.AddListener(OnResetGameDataButton);
            
            _gameDataUi.gameObject.SetActive(false);
            _versionLabel.SetText(Application.version);
        }

        private void OnDisable()
        {
            _playButton.onClick.RemoveListener(OnPlayButtonClicked);
            _showDataButton.onClick.RemoveListener(OnShowDataMenuButtonClicked);
            _hideDataButton.onClick.RemoveListener(OnHideDataMenuButtonClicked);
            _saveGameDataButton.onClick.RemoveListener(OnSaveGameDataButtonClicked);
            _loadGameDataButton.onClick.RemoveListener(OnLoadGameDataButton);
            _resetGameDataButton.onClick.RemoveListener(OnResetGameDataButton);
        }

        private void OnPlayButtonClicked()
        {
            SceneManager.LoadScene(1);
        }

        private void OnShowDataMenuButtonClicked()
        {
            _gameDataUi.gameObject.SetActive(true);
        }

        private void OnHideDataMenuButtonClicked()
        {
            _gameDataUi.gameObject.SetActive(false);
        }

        private void OnSaveGameDataButtonClicked()
        {
            SaveManager.I.SaveGameDataFromString(_gameDataInput.text);
        }

        private void OnLoadGameDataButton()
        {
            _gameDataInput.text = SaveManager.I.GetGameDataAsString();
        }
        
        private void OnResetGameDataButton()
        {
            SaveManager.I.ResetGameData();
            _gameDataInput.text = "";
        }
    }
}