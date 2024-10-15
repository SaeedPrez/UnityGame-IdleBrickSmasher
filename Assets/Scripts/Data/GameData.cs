using System;
using System.Collections.Generic;
using Enums;
using Utilities;

namespace Data
{
    public class GameData
    {
        #region Game

        public string GameId;
        public DateTime GameDataLoadedAt;
        public DateTime GameDataSavedAt;

        #endregion

        #region Settings

        public bool UseENotation;

        #endregion

        #region Level & Experience

        public int LevelCurrent = 1;
        public double TimeCurrentLevel = 0d;
        public double TimeTotal = 0d;

        public double ExperienceCurrent = 0d;
        public double ExperienceRequiredToLevel = 0d;
        private readonly float _experienceGainedPerDamageBase = 0.17f;
        private readonly float _experienceGainedPerHealthBase = 0.09f;
        private readonly float _experienceBase = 20f;
        private readonly float _experienceGrowthPerLevel = 1.07f;

        public double GetExperienceForBrickDamage(double damage)
        {
            return _experienceGainedPerDamageBase * damage;
        }

        public double GetExperienceForBrickDestroyed(double health)
        {
            return _experienceGainedPerHealthBase * health;
        }

        public double GetExperienceNeededToLevel(double level)
        {
            return Helper.CalculateExponentialGrowthCost(EGrowthName.Experience, _experienceBase, _experienceGrowthPerLevel, level);
        }

        #endregion
        
        #region Player

        private readonly float _playerSpeedBase = 2f;
        private readonly float _playerIdleCooldown = 2f;

        public float GetPlayerSpeed()
        {
            return _playerSpeedBase;
        }

        public float GetPlayerIdleCooldown()
        {
            return _playerIdleCooldown;
        }

        #endregion
        
        #region Bricks

        public readonly float BrickNoiseScale = 4;
        private readonly float _brickNoiseSpawnThresholdBase = 0.3f;
        private readonly int _brickMinimumSpawnBase = 3;
        
        private readonly float _brickHealthBase = 0.4f;
        private readonly float _brickHealthGrowthPerLevel = 1.05f;

        private readonly float _brickRowSpawnCooldownBase = 10f;

        public int BrickNoiseSeed = 0;
        public double BrickNoiseOffsetY = 0;
        public double BrickRowLevel = 1;

        public float GetBrickNoiseSpawnThreshold()
        {
            return _brickNoiseSpawnThresholdBase;
        }

        public int GetBrickMinimumSpawnThreshold()
        {
            return _brickMinimumSpawnBase;
        }

        public float GetBrickSpawnCooldown()
        {
            return _brickRowSpawnCooldownBase;
        }
        
        public double GetBrickMaxHealth(double level = -1d)
        {
            if (level == -1d)
                level = BrickRowLevel;

            return Helper.CalculateExponentialGrowthCost(EGrowthName.BrickHealth, _brickHealthBase, _brickHealthGrowthPerLevel, level);
        }

        #endregion

        #region Balls

        public List<BallData> Balls = new()
        {
            new BallData { Id = 1, UnlockLevel = 1 },
            new BallData { Id = 2, UnlockLevel = 2 },
            new BallData { Id = 3, UnlockLevel = 4 },
            new BallData { Id = 4, UnlockLevel = 8 },
            new BallData { Id = 5, UnlockLevel = 12 },
            new BallData { Id = 6, UnlockLevel = 16 },
            new BallData { Id = 7, UnlockLevel = 20 },
            new BallData { Id = 8, UnlockLevel = 25 },
            new BallData { Id = 9, UnlockLevel = 30 },
            new BallData { Id = 10, UnlockLevel = 40 },
            new BallData { Id = 11, UnlockLevel = 50 },
            new BallData { Id = 12, UnlockLevel = 75 },
            new BallData { Id = 13, UnlockLevel = 100 },
            new BallData { Id = 14, UnlockLevel = 150 },
            new BallData { Id = 15, UnlockLevel = 200 },
        };

        private readonly float _ballSpeedBase = 2.5f;
        private readonly float _ballSpeedGrowthPerLevel = 0.1f;
        public readonly int BallSpeedMaxLevel = 10;
        private readonly float _ballDamageBase = 5f;
        private readonly float _ballDamageGrowthPerLevel = 5f;
        public readonly int BallDamageMaxLevel = 10;
        private readonly float _ballCriticalChanceBase = 0f;
        private readonly float _ballCriticalChanceGrowthPerLevel = 0.1f;
        private readonly float _ballCriticalDamageBase = 1f;
        private readonly float _ballCriticalDamagGrowthPerLevel = 0.1f;

        public float GetBallSpeed(Ball ball, int level = -1)
        {
            if (level == -1)
                level = ball.Data.SpeedLevel;
            
            return _ballSpeedBase + level * _ballSpeedGrowthPerLevel;
        }

        public double GetBallDamage(Ball ball, int level = -1)
        {
            if (level == -1)
                level = ball.Data.DamageLevel;
            
            return _ballDamageBase + (level - 1) * _ballDamageGrowthPerLevel;
        }

        #endregion

        #region Active Play

        private readonly int _activePlayHitsBase = 1;
        private readonly int _activePlayHitsGrowthPerLevel = 1;
        private readonly float _activePlayDamageMultiplierBase = 1.5f;
        private readonly float _activePlayExperienceMultiplierBase = 1.5f;

        public int GetActivePlayHits(Ball ball)
        {
            return _activePlayHitsBase;
        }

        public float GetActivePlayDamageMultiplier(Ball ball)
        {
            return _activePlayDamageMultiplierBase;
        }
        
        public float GetActivePlayExpMultiplier(Ball ball)
        {
            return _activePlayExperienceMultiplierBase;
        }
        
        #endregion
        
        #region Coins

        public double CoinsCurrent = 0d;
        private readonly float _coinsPerBrickHealthBase = 0.01f;
        private readonly float _coinsPerLevelBase = 2.5f;

        public double GetCoinsForBrickDestroyed(double maxHealth)
        {
            return _coinsPerBrickHealthBase * maxHealth;
        }

        public double GetCoinsForLeveledUp(double level = -1d)
        {
            if (level == -1d)
                level = LevelCurrent - 1;
            
            return _coinsPerLevelBase * level;
        }

        #endregion

        #region Diamonds

        public double DiamondsCurrent = 0d;
        private readonly float _diamondsPerLevelBase = 0.25f;

        public double GetDiamondsForLeveledUp(double level = -1d)
        {
            if (level == -1d)
                level = LevelCurrent - 1;

            return _diamondsPerLevelBase * level;
        }

        #endregion
    }
}