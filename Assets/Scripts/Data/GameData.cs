using System;
using System.Collections.Generic;
using Core;
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
            return Helper.CalculateExponentialGrowthCost(EStat.Experience, _experienceBase, _experienceGrowthPerLevel, level);
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

            return Helper.CalculateExponentialGrowthCost(EStat.BrickHealth, _brickHealthBase, _brickHealthGrowthPerLevel, level);
        }

        #endregion

        #region Ball

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
        
        #endregion

        #region Ball Speed

        private readonly float _ballSpeedBase = 1.5f;
        private readonly float _ballSpeedGrowthPerLevel = 0.025f;
        private readonly int _ballSpeedMaxLevel = 100;

        public float GetBallSpeed(Ball ball, int level = -1)
        {
            if (level == -1)
                level = ball.Data.SpeedLevel;
            
            return _ballSpeedBase + (_ballSpeedBase * _ballSpeedGrowthPerLevel * (level - 1));
        }

        public bool IsBallSpeedMaxLevel(Ball ball)
        {
            return ball.Data.SpeedLevel >= _ballSpeedMaxLevel;
        }

        public bool CanBallSpeedUpgrade(Ball ball)
        {
            if (IsBallSpeedMaxLevel(ball))
                return false;

            if (CoinsCurrent < 1)
                return false;
            
            return true;
        }

        public void UpgradeBallSpeed(Ball ball)
        {
            if (!CanBallSpeedUpgrade(ball))
                return;
            
            ball.Data.SpeedLevel++;
            EventManager.I.TriggerBallUpgraded(ball, EStat.BallSpeed);
        }

        #endregion

        #region Ball Damage

        private readonly float _ballDamageBase = 5f;
        private readonly float _ballDamageGrowthPerLevel = 5f;
        private readonly int _ballDamageMaxLevel = 100;

        public double GetBallDamage(Ball ball, int level = -1)
        {
            if (level == -1)
                level = ball.Data.DamageLevel;
            
            return _ballDamageBase + ((level - 1) * (level - 1)) * _ballDamageGrowthPerLevel;
        }
        
        public bool IsBallDamageMaxLevel(Ball ball)
        {
            return ball.Data.DamageLevel >= _ballDamageMaxLevel;
        }
        
        public bool CanBallDamageUpgrade(Ball ball)
        {
            if (IsBallDamageMaxLevel(ball))
                return false;

            if (CoinsCurrent < 1)
                return false;
            
            return true;
        }
        
        public void UpgradeBallDamage(Ball ball)
        {
            if (!CanBallDamageUpgrade(ball))
                return;
            
            ball.Data.DamageLevel++;
            EventManager.I.TriggerBallUpgraded(ball, EStat.BallDamage);
        }
        
        #endregion

        #region Ball Critical Chance

        private readonly float _ballCriticalChanceBase = 0f;
        private readonly float _ballCriticalChanceGrowthPerLevel = 1f;
        private readonly int _ballCriticalChanceMaxLevel = 100;

        public double GetBallCriticalChance(Ball ball, int level = -1)
        {
            if (level == -1)
                level = ball.Data.CriticalChanceLevel;
            
            return _ballCriticalChanceBase + ((level - 1) * _ballCriticalChanceGrowthPerLevel);
        }
        
        public bool IsBallCriticalChanceMaxLevel(Ball ball)
        {
            return ball.Data.CriticalChanceLevel >= _ballCriticalChanceMaxLevel;
        }
        
        public bool CanBallCriticalChanceUpgrade(Ball ball)
        {
            if (IsBallCriticalChanceMaxLevel(ball))
                return false;

            if (CoinsCurrent < 1)
                return false;
            
            return true;
        }
        
        public void UpgradeBallCriticalChance(Ball ball)
        {
            if (!CanBallCriticalChanceUpgrade(ball))
                return;
            
            ball.Data.CriticalChanceLevel++;
            EventManager.I.TriggerBallUpgraded(ball, EStat.BallCriticalChance);
        }
        
        #endregion

        #region Ball Critical Damage

        private readonly float _ballCriticalDamageBase = 1f;
        private readonly float _ballCriticalDamageGrowthPerLevel = 0.1f;
        private readonly int _ballCriticalDamageMaxLevel = 1000;

        public double GetBallCriticalDamage(Ball ball, int level = -1)
        {
            if (level == -1)
                level = ball.Data.CriticalDamageLevel;
            
            return _ballCriticalDamageBase + ((level - 1) * _ballCriticalDamageGrowthPerLevel);
        }
        
        public bool IsBallCriticalDamageMaxLevel(Ball ball)
        {
            return ball.Data.CriticalDamageLevel >= _ballCriticalDamageMaxLevel;
        }
        
        public bool CanBallCriticalDamageUpgrade(Ball ball)
        {
            if (IsBallCriticalDamageMaxLevel(ball))
                return false;

            if (CoinsCurrent < 1)
                return false;
            
            return true;
        }
        
        public void UpgradeBallCriticalDamage(Ball ball)
        {
            if (!CanBallCriticalDamageUpgrade(ball))
                return;
            
            ball.Data.CriticalDamageLevel++;
            EventManager.I.TriggerBallUpgraded(ball, EStat.BallCriticalDamage);
        }
        
        #endregion
        
        #region Ball Active Hits

        private readonly int _ballActiveHitsBase = 1;
        private readonly int _ballActiveHitsGrowthPerLevel = 1;
        private readonly int _ballActiveHitsMaxLevel = 100;

        public int GetBallActiveHits(Ball ball, int level = -1)
        {
            if (level == -1)
                level = ball.Data.ActiveHitsLevel;
            
            return _ballActiveHitsBase + _ballActiveHitsGrowthPerLevel * (level - 1);
        }

        public bool IsBallActiveHitsMaxLevel(Ball ball)
        {
            return ball.Data.ActiveHitsLevel >= _ballActiveHitsMaxLevel;
        }

        public bool CanBallActiveHitsUpgrade(Ball ball)
        {
            if (IsBallActiveHitsMaxLevel(ball))
                return false;

            if (CoinsCurrent < 1)
                return false;
            
            return true;
        }

        public void UpgradeBallActiveHits(Ball ball)
        {
            if (!CanBallActiveHitsUpgrade(ball))
                return;
            
            ball.Data.ActiveHitsLevel++;
            EventManager.I.TriggerBallUpgraded(ball, EStat.BallActiveHits);
        }
        
        
        #endregion

        #region Ball Active Damage

        private readonly float _ballActiveDamageBase = 1.5f;
        private readonly float _ballActiveDamageGrowthPerLevel = 0.1f;
        private readonly int _ballActiveDamageMaxLevel = 100;

        public double GetBallActiveDamage(Ball ball, int level = -1)
        {
            if (level == -1)
                level = ball.Data.ActiveDamageLevel;
            
            return _ballActiveDamageBase + ((level - 1) * _ballActiveDamageGrowthPerLevel);
        }
        
        public bool IsBallActiveDamageMaxLevel(Ball ball)
        {
            return ball.Data.ActiveDamageLevel >= _ballActiveDamageMaxLevel;
        }
        
        public bool CanBallActiveDamageUpgrade(Ball ball)
        {
            if (IsBallActiveDamageMaxLevel(ball))
                return false;

            if (CoinsCurrent < 1)
                return false;
            
            return true;
        }
        
        public void UpgradeBallActiveDamage(Ball ball)
        {
            if (!CanBallActiveDamageUpgrade(ball))
                return;
            
            ball.Data.ActiveDamageLevel++;
            EventManager.I.TriggerBallUpgraded(ball, EStat.BallActiveDamage);
        }
        
        #endregion
        
        #region Coins

        public double CoinsCurrent = 0d;
        private readonly float _coinsPerBrickHealthBase = 0.01f;
        private readonly float _coinsPerLevelBase = 2.5f;

        public double GetCoinsForBrickDestroyed(Brick brick)
        {
            return _coinsPerBrickHealthBase * brick.MaxHealth;
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