﻿using System;
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
                LoadOrCreateGameData();
        }

        /// <summary>
        /// Loads or creates game data.
        /// </summary>
        private void LoadOrCreateGameData()
        {
            if (ES3.KeyExists(Constants.SaveGameName))
                LoadGameData();
            else
                CreateGameData();
        }

        /// <summary>
        /// Loads game data.
        /// </summary>
        private void LoadGameData()
        {
            var data = ES3.Load<GameData>(Constants.SaveGameName);
            data.GameDataLoaded = DateTime.UtcNow;
            EventManager.I.TriggerGameDataLoaded(data);
        }

        /// <summary>
        /// Creates a new game data.
        /// </summary>
        private void CreateGameData()
        {
            var data = new GameData();
            EventManager.I.TriggerGameDataLoaded(data);
        }

        /// <summary>
        /// Saves game data.
        /// </summary>
        private void SaveGameData()
        {
            GameManager.Data.GameDataSaved = DateTime.UtcNow;
            
            // TODO: Improve
            GameManager.Data.BrickNoiseOffsetY -= 21;

            if (GameManager.Data.BrickNoiseOffsetY < 0)
                GameManager.Data.BrickNoiseOffsetY = 0;

            GameManager.Data.BrickRowsSpawned -= 19;

            if (GameManager.Data.BrickRowsSpawned < 0)
                GameManager.Data.BrickRowsSpawned = 0;
            
            ES3.Save(Constants.SaveGameName, GameManager.Data);
            EventManager.I.TriggerGameDataSaved(GameManager.Data);
        }
    }
}