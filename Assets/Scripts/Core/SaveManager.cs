using Data;
using Enums;
using UnityEngine;

namespace Core
{
    public class SaveManager : MonoBehaviour
    {
        private void OnEnable()
        {
            EventManager.I.OnGameStateChanged += OnGameStateChanged;
        }
        
        private void OnDisable()
        {
            EventManager.I.OnGameStateChanged -= OnGameStateChanged;
            SaveGameData();
        }
        
        private void OnGameStateChanged(EGameState state)
        {
            if (state is EGameState.Loading)
                TryLoadGameData();
        }

        private void TryLoadGameData()
        {
            if (ES3.KeyExists(Constants.SaveGameName))
                LoadGameData();
            else
                CreateGameData();
        }

        private void LoadGameData()
        {
            var data = ES3.Load<GameData>(Constants.SaveGameName);
            EventManager.I.TriggerGameDataLoaded(data);
        }

        private void CreateGameData()
        {
            var data = new GameData();
            EventManager.I.TriggerGameDataLoaded(data);
        }

        private void SaveGameData()
        {
            // TODO: Improve
            GameManager.Data.BrickNoiseOffsetY -= 21;

            if (GameManager.Data.BrickNoiseOffsetY < 0)
                GameManager.Data.BrickNoiseOffsetY = 0;
            
            ES3.Save(Constants.SaveGameName, GameManager.Data);
        }
    }
}