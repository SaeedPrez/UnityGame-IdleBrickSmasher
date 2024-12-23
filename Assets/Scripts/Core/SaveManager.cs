using System;
using System.Collections;
using System.IO;
using Prez.Data;
using Prez.Enums;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Prez.Core
{
    public class SaveManager : MonoBehaviour
    {
        [SerializeField] private float _saveInterval;
        
        public static SaveManager I { get; private set; }

        private string _filePath;

        private void Awake()
        {
            SetupSingleton();
        }
        
        private void OnEnable()
        {
            EventManager.I.OnGameStateChanged += OnGameStateChanged;
            StartCoroutine(Save());

            _filePath = Application.persistentDataPath + "/" + "SaveFile.es3";
        }

        private void OnDisable()
        {
            EventManager.I.OnGameStateChanged -= OnGameStateChanged;
            StopAllCoroutines();
        }

        private void OnGameStateChanged(EGameState state)
        {
            if (state is EGameState.Loading)
                LoadOrCreateGameData();
        }

        #region Singleton

        /// <summary>
        ///     Setups the singleton.
        /// </summary>
        private void SetupSingleton()
        {
            if (I)
            {
                Destroy(gameObject);
                return;
            }

            I = this;
        }

        #endregion
        
        /// <summary>
        ///     Loads or creates game data.
        /// </summary>
        private void LoadOrCreateGameData()
        {
            if (ES3.KeyExists(Constants.SaveGameName))
                LoadGameData();
            else
                CreateGameData();
        }

        /// <summary>
        ///     Loads game data.
        /// </summary>
        private void LoadGameData()
        {
            var data = ES3.Load<GameData>(Constants.SaveGameName);
            data.GameDataLoadedAt = DateTime.UtcNow;
            EventManager.I.TriggerGameDataLoaded(data);
        }

        /// <summary>
        ///     Creates a new game data.
        /// </summary>
        private void CreateGameData()
        {
            var data = new GameData();
            EventManager.I.TriggerGameDataLoaded(data);
        }

        /// <summary>
        /// Save game after interval.
        /// </summary>
        /// <returns></returns>
        private IEnumerator Save()
        {
            while (true)
            {
                yield return new WaitForSeconds(_saveInterval);
                
                if (GameManager.State is EGameState.Playing)
                    SaveGameData();
            }
        }
        
        /// <summary>
        ///     Saves game data.
        /// </summary>
        private void SaveGameData()
        {
            GameManager.Data.GameDataSavedAt = DateTime.UtcNow;

            // TODO: Improve
            GameManager.Data.BrickNoiseOffsetY -= 21;

            if (GameManager.Data.BrickNoiseOffsetY < 0)
                GameManager.Data.BrickNoiseOffsetY = 0;

            GameManager.Data.BrickRowLevel -= 21;

            if (GameManager.Data.BrickRowLevel < 1)
                GameManager.Data.BrickRowLevel = 1;

            ES3.Save(Constants.SaveGameName, GameManager.Data);
            EventManager.I.TriggerGameDataSaved(GameManager.Data);
        }

        public string GetGameDataAsString()
        {
            SaveGameData();
            return Convert.ToBase64String(File.ReadAllBytes(_filePath));
        }

        public void SaveGameDataFromString(string data)
        {
            File.WriteAllBytes(_filePath, Convert.FromBase64String(data));
            SceneManager.LoadScene(0);
        }

        public void Reset()
        {
            File.Delete(_filePath);
            SceneManager.LoadScene(0);
        }
    }
}