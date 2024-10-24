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

        private readonly float _experienceBase = 10f;
        private readonly float _experienceGrowthPerLevel = 1.04f;
        private readonly float _experienceGainedPerDamageBase = 0.17f;
        private readonly float _experienceGainedPerHealthBase = 0.09f;
        public double ExperienceCurrent = 0d;
        public double ExperienceRequiredToLevel = 0d;

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
        
        private readonly float _brickHealthBase = 0.5f;
        private readonly float _brickHealthGrowthPerLevel = 1.025f;

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
            new BallData { Id = 3, UnlockLevel = 10 },
            new BallData { Id = 4, UnlockLevel = 25 },
            new BallData { Id = 5, UnlockLevel = 50 },
            new BallData { Id = 6, UnlockLevel = 100 },
            // new BallData { Id = 7, UnlockLevel = 250 },
            // new BallData { Id = 8, UnlockLevel = 500 },
            // new BallData { Id = 9, UnlockLevel = 1000 },
            // new BallData { Id = 10, UnlockLevel = 40 },
            // new BallData { Id = 11, UnlockLevel = 50 },
            // new BallData { Id = 12, UnlockLevel = 75 },
            // new BallData { Id = 13, UnlockLevel = 100 },
            // new BallData { Id = 14, UnlockLevel = 150 },
            // new BallData { Id = 15, UnlockLevel = 200 },
        };
        
        #endregion

        #region Ball Speed

        private readonly float _ballSpeedBase = 1.5f;
        private readonly float _ballSpeedGrowthPerLevel = 0.025f;
        private readonly int _ballSpeedMaxLevel = 100;
        private readonly float _ballSpeedCostBase = 1.0f;
        private readonly float _ballSpeedCostGrowthPerLevel = 0.25f;

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

            if (UpgradePointsCurrent < GetBallSpeedUpgradeCost(ball))
                return false;
            
            return true;
        }

        public double GetBallSpeedUpgradeCost(Ball ball, int level = -1)
        {
            if (level == -1)
                level = ball.Data.SpeedLevel;

            return _ballSpeedCostBase + (_ballSpeedCostGrowthPerLevel * (level - 1));
        }

        public void UpgradeBallSpeed(Ball ball)
        {
            if (!CanBallSpeedUpgrade(ball))
                return;

            var cost = GetBallSpeedUpgradeCost(ball);
            
            ball.Data.SpeedLevel++;
            EventManager.I.TriggerBallUpgraded(ball, EStat.BallSpeed, cost);
        }

        #endregion

        #region Ball Damage

        private readonly float _ballDamageBase = 5f;
        private readonly float _ballDamageGrowthPerLevel = 3.35f;
        private readonly int _ballDamageMaxLevel = 100;
        private readonly float _ballDamageCostBase = 1.0f;
        private readonly float _ballDamageCostGrowthPerLevel = 0.25f;
        
        public double GetBallDamage(Ball ball, int level = -1)
        {
            if (level == -1)
                level = ball.Data.DamageLevel;
            
            return _ballDamageBase + Math.Pow(level, _ballDamageGrowthPerLevel);
        }
        
        public bool IsBallDamageMaxLevel(Ball ball)
        {
            return ball.Data.DamageLevel >= _ballDamageMaxLevel;
        }
        
        public bool CanBallDamageUpgrade(Ball ball)
        {
            if (IsBallDamageMaxLevel(ball))
                return false;

            if (UpgradePointsCurrent < GetBallDamageUpgradeCost(ball))
                return false;
            
            return true;
        }
        
        public double GetBallDamageUpgradeCost(Ball ball, int level = -1)
        {
            if (level == -1)
                level = ball.Data.DamageLevel;

            return _ballDamageCostBase + (_ballDamageCostGrowthPerLevel * (level - 1));
        }
        
        public void UpgradeBallDamage(Ball ball)
        {
            if (!CanBallDamageUpgrade(ball))
                return;

            var cost = GetBallDamageUpgradeCost(ball);
            
            ball.Data.DamageLevel++;
            EventManager.I.TriggerBallUpgraded(ball, EStat.BallDamage, cost);
        }
        
        #endregion

        #region Ball Critical Chance

        private readonly float _ballCriticalChanceBase = 0f;
        private readonly float _ballCriticalChanceGrowthPerLevel = 1f;
        private readonly int _ballCriticalChanceMaxLevel = 100;
        private readonly float _ballCriticalChanceCostBase = 1.0f;
        private readonly float _ballCriticalChanceCostGrowthPerLevel = 0.25f;
        
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

            if (UpgradePointsCurrent < GetBallCriticalChanceUpgradeCost(ball))
                return false;
            
            return true;
        }
        
        public double GetBallCriticalChanceUpgradeCost(Ball ball, int level = -1)
        {
            if (level == -1)
                level = ball.Data.CriticalChanceLevel;

            return _ballCriticalChanceCostBase + (_ballCriticalChanceCostGrowthPerLevel * (level - 1));
        }
        
        public void UpgradeBallCriticalChance(Ball ball)
        {
            if (!CanBallCriticalChanceUpgrade(ball))
                return;
            
            var cost = GetBallCriticalChanceUpgradeCost(ball);

            ball.Data.CriticalChanceLevel++;
            EventManager.I.TriggerBallUpgraded(ball, EStat.BallCriticalChance, cost);
        }
        
        #endregion

        #region Ball Critical Damage

        private readonly float _ballCriticalDamageBase = 1f;
        private readonly float _ballCriticalDamageGrowthPerLevel = 0.1f;
        private readonly int _ballCriticalDamageMaxLevel = 1000;
        private readonly float _ballCriticalDamageCostBase = 1.0f;
        private readonly float _ballCriticalDamageCostGrowthPerLevel = 0.25f;

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

            if (UpgradePointsCurrent < GetBallCriticalDamageUpgradeCost(ball))
                return false;
            
            return true;
        }
        
        public double GetBallCriticalDamageUpgradeCost(Ball ball, int level = -1)
        {
            if (level == -1)
                level = ball.Data.CriticalDamageLevel;

            return _ballCriticalDamageCostBase + (_ballCriticalDamageCostGrowthPerLevel * (level - 1));
        }
        
        public void UpgradeBallCriticalDamage(Ball ball)
        {
            if (!CanBallCriticalDamageUpgrade(ball))
                return;
            
            var cost = GetBallCriticalDamageUpgradeCost(ball);

            ball.Data.CriticalDamageLevel++;
            EventManager.I.TriggerBallUpgraded(ball, EStat.BallCriticalDamage, cost);
        }
        
        #endregion
        
        #region Ball Active Hits

        private readonly int _ballActiveHitsBase = 1;
        private readonly int _ballActiveHitsGrowthPerLevel = 1;
        private readonly int _ballActiveHitsMaxLevel = 100;
        private readonly float _ballActiveHitsCostBase = 1.0f;
        private readonly float _ballActiveHitsCostGrowthPerLevel = 0.25f;
        
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

            if (UpgradePointsCurrent < 1)
                return false;
            
            return true;
        }

        public double GetBallActiveHitsUpgradeCost(Ball ball, int level = -1)
        {
            if (level == -1)
                level = ball.Data.ActiveHitsLevel;

            return _ballActiveHitsCostBase + (_ballActiveHitsCostGrowthPerLevel * (level - 1));
        }
        
        public void UpgradeBallActiveHits(Ball ball)
        {
            if (!CanBallActiveHitsUpgrade(ball))
                return;
            
            var cost = GetBallActiveHitsUpgradeCost(ball);

            ball.Data.ActiveHitsLevel++;
            EventManager.I.TriggerBallUpgraded(ball, EStat.BallActiveHits, cost);
        }
        
        
        #endregion

        #region Ball Active Damage

        private readonly float _ballActiveDamageBase = 1.5f;
        private readonly float _ballActiveDamageGrowthPerLevel = 0.1f;
        private readonly int _ballActiveDamageMaxLevel = 100;
        private readonly float _ballActiveDamageCostBase = 1.0f;
        private readonly float _ballActiveDamageCostGrowthPerLevel = 0.25f;
        
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

            if (UpgradePointsCurrent < GetBallActiveDamageUpgradeCost(ball))
                return false;
            
            return true;
        }
        
        public double GetBallActiveDamageUpgradeCost(Ball ball, int level = -1)
        {
            if (level == -1)
                level = ball.Data.ActiveDamageLevel;

            return _ballActiveDamageCostBase + (_ballActiveDamageCostGrowthPerLevel * (level - 1));
        }
        
        public void UpgradeBallActiveDamage(Ball ball)
        {
            if (!CanBallActiveDamageUpgrade(ball))
                return;
            
            var cost = GetBallActiveDamageUpgradeCost(ball);

            ball.Data.ActiveDamageLevel++;
            EventManager.I.TriggerBallUpgraded(ball, EStat.BallActiveDamage, cost);
        }
        
        #endregion
        
        #region Update Points

        public double UpgradePointsCurrent = 0d;

        public double GetUpgradePointsForLevel(double level = -1)
        {
            if (level == -1)
                level = LevelCurrent - 1;

            return level switch
            {
                >= 100 => 3,
                >= 50 => 2,
                >= 25 => 1.5,
                >= 10 => 1.25,
                _ => 1
            };
        }
        
        #endregion

        #region Talent Points

        public double TalentPointsCurrent = 0d;
        private readonly double _talentPointsBase = 1d;

        public double GetDiamondsForLeveledUp(double level = -1d)
        {
            if (level == -1)
                level = LevelCurrent - 1;

            return level switch
            {
                >= 100 => 1,
                _ => 0
            };
        }

        #endregion
    }
}