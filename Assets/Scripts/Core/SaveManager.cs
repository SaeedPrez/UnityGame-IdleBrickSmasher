﻿using System;
using System.Collections;
using System.Text;
using DG.Tweening;
using Prez.Data;
using Prez.Enums;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

namespace Prez.Core
{
    public class SaveManager : MonoBehaviour
    {
        [SerializeField] private float _saveInterval;
        
        public static SaveManager I { get; private set; }

        private void Awake()
        {
            SetupSingleton();
        }
        
        private void OnEnable()
        {
            EventManager.I.OnGameStateChanged += OnGameStateChanged;

            if (_saveInterval > 0)
                StartCoroutine(Save());
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

            // TODO: Improve
            data.BrickNoiseOffsetY -= 21;

            if (data.BrickNoiseOffsetY < 0)
                data.BrickNoiseOffsetY = 0;

            data.BrickRowLevel -= 21;

            if (data.BrickRowLevel < 1)
                data.BrickRowLevel = 1;
            
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
        private void SaveGameData(GameData data = null)
        {
            data ??= GameManager.Data;
            data.GameDataSavedAt = DateTime.UtcNow;
            
            ES3.Save(Constants.SaveGameName, data);
            EventManager.I.TriggerGameDataSaved(data);
        }

        public string GetGameDataAsString()
        {
            try
            {
                var data = ES3.Load<GameData>(Constants.SaveGameName);
                var dataJson = JsonConvert.SerializeObject(data);
                var dataEncrypted = ES3.EncryptString(dataJson, Constants.SaveGameName);
                var dataBytes = Encoding.UTF8.GetBytes(dataEncrypted);
                return Convert.ToBase64String(dataBytes);
            }
            catch (Exception e)
            {
                MessageManager.Queue("Game data could not be loaded");
                return "";
            }
        }

        public void SaveGameDataFromString(string dataSaved)
        {
            try
            {
                var dataBytes = Convert.FromBase64String(dataSaved);
                var dataEncrypted = Encoding.UTF8.GetString(dataBytes);
                var dataJson = ES3.DecryptString(dataEncrypted, Constants.SaveGameName);
                var data = JsonConvert.DeserializeObject<GameData>(dataJson, new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace });

                SaveGameData(data);
                MessageManager.Queue("Game data has been saved");
            }
            catch (Exception e)
            {
                MessageManager.Queue("Game data could not be saved");
            }
        }

        public void Reset()
        {
            SaveGameData(new GameData());
            MessageManager.Queue("Game data has been reset.");
        }
    }
}