using DG.Tweening;
using Prez.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Prez.Menus
{
    public class SettingsMenu : MenuBase
    {
        [SerializeField] private Button _startMenuButton;

        private void OnEnable()
        {
            _startMenuButton.onClick.AddListener(OnStartMenuButtonClicked);
        }

        private void OnDisable()
        {
            _startMenuButton.onClick.RemoveListener(OnStartMenuButtonClicked);
        }

        private void OnStartMenuButtonClicked()
        {
            DOTween.KillAll();
            SaveManager.I.SaveGameData();
            SceneManager.LoadScene(0);
        }
    }
}