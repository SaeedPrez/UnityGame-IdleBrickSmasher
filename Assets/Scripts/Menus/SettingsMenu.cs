using Prez.Core;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Prez.Menus
{
    public class SettingsMenu : MenuBase
    {
        [SerializeField] private Button _discordButton;
        [SerializeField] private Button _websiteButton;
        [SerializeField] private TMP_InputField _gameDataUi;
        [SerializeField] private Button _loadGameDataButton;
        [SerializeField] private Button _saveGameDataButton;
        [SerializeField] private Button _resetGameDataButton;

        private void OnEnable()
        {
            _discordButton.onClick.AddListener(OnDiscordButtonClicked);
            _websiteButton.onClick.AddListener(OnWebsiteButtonClicked);
            _saveGameDataButton.onClick.AddListener(OnSaveGameDataButtonClicked);
            _loadGameDataButton.onClick.AddListener(OnLoadGameDataButton);
            _resetGameDataButton.onClick.AddListener(OnResetGameDataButton);
        }

        private void OnDisable()
        {
            _discordButton.onClick.RemoveListener(OnDiscordButtonClicked);
            _websiteButton.onClick.RemoveListener(OnWebsiteButtonClicked);
            _saveGameDataButton.onClick.RemoveListener(OnSaveGameDataButtonClicked);
            _loadGameDataButton.onClick.RemoveListener(OnLoadGameDataButton);
            _resetGameDataButton.onClick.RemoveListener(OnResetGameDataButton);
        }
        
        private void OnDiscordButtonClicked()
        {
            Application.OpenURL("https://mrprezdev.com/discord");
        }

        private void OnWebsiteButtonClicked()
        {
            Application.OpenURL("https://mrprezdev.com");
        }
        
        private void OnSaveGameDataButtonClicked()
        {
            SaveManager.I.SaveGameDataFromString(_gameDataUi.text);
        }

        private void OnLoadGameDataButton()
        {
            _gameDataUi.text = SaveManager.I.GetGameDataAsString();
        }
        
        private void OnResetGameDataButton()
        {
            SaveManager.I.Reset();
        }
    }
}