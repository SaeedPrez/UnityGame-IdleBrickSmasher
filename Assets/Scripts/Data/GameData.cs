using System;
using System.Collections.Generic;
using Prez.Core;
using Prez.Enums;
using Prez.Utilities;

namespace Prez.Data
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

        private readonly float _experienceGainedPerBrickBase = 0.1f;
        public double ExperienceCurrent;
        public double ExperienceRequiredToLevel = 0d;

        private double GetExperienceForBrickDamage(DamageData data)
        {
            if (!data.BrickDestroyed)
                return 0;

            return _experienceGainedPerBrickBase * data.Brick.SpawnedRowNumber;
        }

        public double GetExperienceNeededToLevel(int level)
        {
            return Numbers.GetLevelExperience(level - 1);
        }

        public void AddBrickDamagedExperience(DamageData data)
        {
            data.Experience = GetExperienceForBrickDamage(data);
            ExperienceCurrent += data.Experience;
        }

        #endregion

        #region Paddle Cooldown

        private readonly float _paddleIdleCooldown = 10f;

        public float GetPaddleIdleCooldown()
        {
            return _paddleIdleCooldown;
        }

        #endregion
        
        #region Paddle Speed

        public int PaddleSpeedLevel = 1;
        private readonly float _paddleSpeedBase = 1f;
        private readonly float _paddleSpeedGrowthPerLevel = 0.0101f;
        private readonly int _paddleSpeedMaxLevel = 100;
        private readonly float _paddleSpeedCostBase = 1.0f;
        private readonly float _paddleSpeedCostGrowthPerLevel = 0.25f;
        
        public float GetPaddleSpeed(int level = -1)
        {
            if (level == -1)
                level = PaddleSpeedLevel;
            
            return _paddleSpeedBase + _paddleSpeedGrowthPerLevel * (level - 1);
        }

        public bool IsPaddleSpeedMaxLevel()
        {
            return PaddleSpeedLevel >= _paddleSpeedMaxLevel;
        }

        public bool CanPaddleSpeedUpgrade()
        {
            if (IsPaddleSpeedMaxLevel())
                return false;

            if (UpgradePointsCurrent < GetPaddleSpeedUpgradeCost())
                return false;

            return true;
        }
        
        public float GetPaddleSpeedUpgradeCost(int level = -1)
        {
            if (level == -1)
                level = PaddleSpeedLevel;

            return _paddleSpeedCostBase + _paddleSpeedCostGrowthPerLevel * (level - 1);
        }

        public void UpgradePaddleSpeed()
        {
            if (!CanPaddleSpeedUpgrade())
                return;

            var cost = GetPaddleSpeedUpgradeCost();

            PaddleSpeedLevel++;
            EventManager.I.TriggerPaddleUpgraded(EStat.Speed, cost);
        }
        
        #endregion

        #region Paddle Bullet Damage

        public int PaddleBulletDamageLevel = 1;
        private readonly float _paddleBulletDamageBase = 2f;
        private readonly float _paddleBulletDamageGrowthPerLevel = 2f;
        private readonly int _paddleBulletDamageMaxLevel = 1000;
        private readonly float _paddleBulletDamageCostBase = 1.0f;
        private readonly float _paddleBulletDamageCostGrowthPerLevel = 0.25f;
        
        public float GetPaddleBulletDamage(int level = -1)
        {
            if (level == -1)
                level = PaddleBulletDamageLevel;
            
            return _paddleBulletDamageBase + _paddleBulletDamageGrowthPerLevel * (level - 1);
        }
        
        public bool IsPaddleBulletDamageMaxLevel()
        {
            return PaddleBulletDamageLevel >= _paddleBulletDamageMaxLevel;
        }

        public bool CanPaddleBulletDamageUpgrade()
        {
            if (IsPaddleBulletDamageMaxLevel())
                return false;

            if (UpgradePointsCurrent < GetPaddleBulletDamageUpgradeCost())
                return false;

            return true;
        }

        public float GetPaddleBulletDamageUpgradeCost(int level = -1)
        {
            if (level == -1)
                level = PaddleBulletDamageLevel;

            return _paddleBulletDamageCostBase + _paddleBulletDamageCostGrowthPerLevel * (level - 1);
        }

        public void UpgradePaddleBulletDamage()
        {
            if (!CanPaddleBulletDamageUpgrade())
                return;

            var cost = GetPaddleBulletDamageUpgradeCost();

            PaddleBulletDamageLevel++;
            EventManager.I.TriggerPaddleBulletUpgraded(EStat.Speed, cost);
        }

        #endregion

        #region Paddle Bullet Critical Chance

        public int PaddleBulletCriticalChanceLevel = 1;
        private readonly float _paddleBulletCriticalChanceBase = 0f;
        private readonly float _paddleBulletCriticalChanceGrowthPerLevel = 1.0101f;
        private readonly int _paddleBulletCriticalChanceMaxLevel = 100;
        private readonly float _paddleBulletCriticalChanceCostBase = 1.0f;
        private readonly float _paddleBulletCriticalChanceCostGrowthPerLevel = 0.25f;

        public float GetPaddleBulletCriticalChance(int level = -1)
        {
            if (level == -1)
                level = PaddleBulletCriticalChanceLevel;

            return _paddleBulletCriticalChanceBase + _paddleBulletCriticalChanceGrowthPerLevel * (level - 1);
        }

        public bool IsPaddleBulletCriticalChanceMaxLevel()
        {
            return PaddleBulletCriticalChanceLevel >= _paddleBulletCriticalChanceMaxLevel;
        }

        public bool CanPaddleBulletCriticalChanceUpgrade()
        {
            if (IsPaddleBulletCriticalChanceMaxLevel())
                return false;

            if (UpgradePointsCurrent < GetPaddleBulletCriticalChanceUpgradeCost())
                return false;

            return true;
        }

        public float GetPaddleBulletCriticalChanceUpgradeCost(int level = -1)
        {
            if (level == -1)
                level = PaddleBulletCriticalChanceLevel;

            return _paddleBulletCriticalChanceCostBase + _paddleBulletCriticalChanceCostGrowthPerLevel * (level - 1);
        }

        public void UpgradePaddleBulletCriticalChance()
        {
            if (!CanPaddleBulletCriticalChanceUpgrade())
                return;

            var cost = GetPaddleBulletCriticalChanceUpgradeCost();

            PaddleBulletCriticalChanceLevel++;
            EventManager.I.TriggerPaddleBulletUpgraded(EStat.CriticalChance, cost);
        }
        
        #endregion

        #region Paddle Bullet Critical Damage

        public int PaddleBulletCriticalDamageLevel = 1;
        private readonly float _paddleBulletCriticalDamageBase = 1f;
        private readonly float _paddleBulletCriticalDamageGrowthPerLevel = 0.1f;
        private readonly int _paddleBulletCriticalDamageMaxLevel = 1000;
        private readonly float _paddleBulletCriticalDamageCostBase = 1.0f;
        private readonly float _paddleBulletCriticalDamageCostGrowthPerLevel = 0.25f;

        public double GetPaddleBulletCriticalDamage(int level = -1)
        {
            if (level == -1)
                level = PaddleBulletCriticalDamageLevel;

            return _paddleBulletCriticalDamageBase +  _paddleBulletCriticalDamageGrowthPerLevel * (level - 1);
        }

        public bool IsPaddleBulletCriticalDamageMaxLevel()
        {
            return PaddleBulletCriticalDamageLevel >= _paddleBulletCriticalDamageMaxLevel;
        }

        public bool CanPaddleBulletCriticalDamageUpgrade()
        {
            if (IsPaddleBulletCriticalDamageMaxLevel())
                return false;

            if (UpgradePointsCurrent < GetPaddleBulletCriticalDamageUpgradeCost())
                return false;

            return true;
        }

        public float GetPaddleBulletCriticalDamageUpgradeCost(int level = -1)
        {
            if (level == -1)
                level = PaddleBulletCriticalDamageLevel;

            return _paddleBulletCriticalDamageCostBase + _paddleBulletCriticalDamageCostGrowthPerLevel * (level - 1);
        }

        public void UpgradePaddleBulletCriticalDamage()
        {
            if (!CanPaddleBulletCriticalDamageUpgrade())
                return;

            var cost = GetPaddleBulletCriticalDamageUpgradeCost();

            PaddleBulletCriticalDamageLevel++;
            EventManager.I.TriggerPaddleBulletUpgraded(EStat.CriticalDamage, cost);
        }

        #endregion
        
        #region Paddle Fire Rate

        public int PaddleBulletFireRateLevel = 1;
        private readonly float _paddleBulletFireRateBase = 0.2f;
        private readonly float _paddleBulletFireRateGrowthPerLevel = 0.0909f;
        private readonly int _paddleBulletFireRateMaxLevel = 100;
        private readonly float _paddleBulletFireRateCostBase = 1.0f;
        private readonly float _paddleBulletFireRateCostGrowthPerLevel = 0.25f;
        
        public float GetPaddleBulletFireRate(int level = -1)
        {
            if (level == -1)
                level = PaddleBulletFireRateLevel;
            
            return _paddleBulletFireRateBase + _paddleBulletFireRateGrowthPerLevel * (level - 1);
        }

        public bool IsPaddleBulletFireRateMaxLevel()
        {
            return PaddleBulletFireRateLevel >= _paddleBulletFireRateMaxLevel;
        }

        public bool CanPaddleBulletFireRateUpgrade()
        {
            if (IsPaddleBulletFireRateMaxLevel())
                return false;

            if (UpgradePointsCurrent < GetPaddleBulletFireRateUpgradeCost())
                return false;

            return true;
        }
        
        public float GetPaddleBulletFireRateUpgradeCost(int level = -1)
        {
            if (level == -1)
                level = PaddleBulletFireRateLevel;

            return _paddleBulletFireRateCostBase + _paddleBulletFireRateCostGrowthPerLevel * (level - 1);
        }

        public void UpgradePaddleBulletFireRate()
        {
            if (!CanPaddleBulletFireRateUpgrade())
                return;

            var cost = GetPaddleBulletFireRateUpgradeCost();

            PaddleBulletFireRateLevel++;
            EventManager.I.TriggerPaddleBulletUpgraded(EStat.Speed, cost);
        }

        #endregion
        
        #region Paddle Bullet Speed

        public int PaddleBulletSpeedLevel = 1;
        private readonly float _paddleBulletSpeedBase = 1.5f;
        private readonly float _paddleBulletSpeedGrowthPerLevel = 0.0859f;
        private readonly int _paddleBulletSpeedMaxLevel = 100;
        private readonly float _paddleBulletSpeedCostBase = 1.0f;
        private readonly float _paddleBulletSpeedCostGrowthPerLevel = 0.25f;
        
        public float GetPaddleBulletSpeed(int level = -1)
        {
            if (level == -1)
                level = PaddleBulletSpeedLevel;
            
            return _paddleBulletSpeedBase + _paddleBulletSpeedGrowthPerLevel * (level - 1);
        }

        public bool IsPaddleBulletSpeedMaxLevel()
        {
            return PaddleBulletSpeedLevel >= _paddleBulletSpeedMaxLevel;
        }

        public bool CanPaddleBulletSpeedUpgrade()
        {
            if (IsPaddleBulletSpeedMaxLevel())
                return false;

            if (UpgradePointsCurrent < GetPaddleBulletSpeedUpgradeCost())
                return false;

            return true;
        }
        
        public float GetPaddleBulletSpeedUpgradeCost(int level = -1)
        {
            if (level == -1)
                level = PaddleBulletSpeedLevel;

            return _paddleBulletSpeedCostBase + _paddleBulletSpeedCostGrowthPerLevel * (level - 1);
        }

        public void UpgradePaddleBulletSpeed()
        {
            if (!CanPaddleBulletSpeedUpgrade())
                return;

            var cost = GetPaddleBulletSpeedUpgradeCost();

            PaddleBulletSpeedLevel++;
            EventManager.I.TriggerPaddleBulletUpgraded(EStat.Speed, cost);
        }
        
        #endregion

        #region Bricks

        public readonly float BrickNoiseScale = 4;
        private readonly float _brickNoiseSpawnThresholdBase = 0.3f;
        private readonly int _brickMinimumSpawnBase = 10;

        private readonly float _brickHealthBase = 1f;
        private readonly float _brickHealthGrowthPerLevel = 0.25f;

        private readonly float _brickRowSpawnCooldownBase = 11f;

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

            return _brickHealthBase + _brickHealthGrowthPerLevel * level;
        }

        #endregion

        #region Balls

        public List<BallData> Balls = new()
        {
            new BallData { Id = 1, UnlockLevel = 1 },
            new BallData { Id = 2, UnlockLevel = 5 },
            new BallData { Id = 3, UnlockLevel = 20 },
            new BallData { Id = 4, UnlockLevel = 50 },
            new BallData { Id = 5, UnlockLevel = 100 },
            // new BallData { Id = 6, UnlockLevel = 100 }
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

            return _ballSpeedBase + _ballSpeedGrowthPerLevel * (level - 1);
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

        public float GetBallSpeedUpgradeCost(Ball ball, int level = -1)
        {
            if (level == -1)
                level = ball.Data.SpeedLevel;

            return _ballSpeedCostBase + _ballSpeedCostGrowthPerLevel * (level - 1);
        }

        public void UpgradeBallSpeed(Ball ball)
        {
            if (!CanBallSpeedUpgrade(ball))
                return;

            var cost = GetBallSpeedUpgradeCost(ball);

            ball.Data.SpeedLevel++;
            EventManager.I.TriggerBallUpgraded(ball, EStat.Speed, cost);
        }

        #endregion

        #region Ball Damage

        private readonly float _ballDamageBase = 2f;
        private readonly float _ballDamageGrowthPerLevel = 2f;
        private readonly int _ballDamageMaxLevel = 1000;
        private readonly float _ballDamageCostBase = 1.0f;
        private readonly float _ballDamageCostGrowthPerLevel = 0.25f;

        public double GetBallDamage(Ball ball, int level = -1)
        {
            if (level == -1)
                level = ball.Data.DamageLevel;

            return _ballDamageBase + _ballDamageGrowthPerLevel * (level - 1);
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

        public float GetBallDamageUpgradeCost(Ball ball, int level = -1)
        {
            if (level == -1)
                level = ball.Data.DamageLevel;

            return _ballDamageCostBase + _ballDamageCostGrowthPerLevel * (level - 1);
        }

        public void UpgradeBallDamage(Ball ball)
        {
            if (!CanBallDamageUpgrade(ball))
                return;

            var cost = GetBallDamageUpgradeCost(ball);

            ball.Data.DamageLevel++;
            EventManager.I.TriggerBallUpgraded(ball, EStat.Damage, cost);
        }

        #endregion

        #region Ball Critical Chance

        private readonly float _ballCriticalChanceBase = 0f;
        private readonly float _ballCriticalChanceGrowthPerLevel = 1f;
        private readonly int _ballCriticalChanceMaxLevel = 100;
        private readonly float _ballCriticalChanceCostBase = 1.0f;
        private readonly float _ballCriticalChanceCostGrowthPerLevel = 0.25f;

        public float GetBallCriticalChance(Ball ball, int level = -1)
        {
            if (level == -1)
                level = ball.Data.CriticalChanceLevel;

            return _ballCriticalChanceBase + (level - 1) * _ballCriticalChanceGrowthPerLevel;
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

        public float GetBallCriticalChanceUpgradeCost(Ball ball, int level = -1)
        {
            if (level == -1)
                level = ball.Data.CriticalChanceLevel;

            return _ballCriticalChanceCostBase + _ballCriticalChanceCostGrowthPerLevel * (level - 1);
        }

        public void UpgradeBallCriticalChance(Ball ball)
        {
            if (!CanBallCriticalChanceUpgrade(ball))
                return;

            var cost = GetBallCriticalChanceUpgradeCost(ball);

            ball.Data.CriticalChanceLevel++;
            EventManager.I.TriggerBallUpgraded(ball, EStat.CriticalChance, cost);
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

            return _ballCriticalDamageBase + (level - 1) * _ballCriticalDamageGrowthPerLevel;
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

        public float GetBallCriticalDamageUpgradeCost(Ball ball, int level = -1)
        {
            if (level == -1)
                level = ball.Data.CriticalDamageLevel;

            return _ballCriticalDamageCostBase + _ballCriticalDamageCostGrowthPerLevel * (level - 1);
        }

        public void UpgradeBallCriticalDamage(Ball ball)
        {
            if (!CanBallCriticalDamageUpgrade(ball))
                return;

            var cost = GetBallCriticalDamageUpgradeCost(ball);

            ball.Data.CriticalDamageLevel++;
            EventManager.I.TriggerBallUpgraded(ball, EStat.CriticalDamage, cost);
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

        public float GetBallActiveHitsUpgradeCost(Ball ball, int level = -1)
        {
            if (level == -1)
                level = ball.Data.ActiveHitsLevel;

            return _ballActiveHitsCostBase + _ballActiveHitsCostGrowthPerLevel * (level - 1);
        }

        public void UpgradeBallActiveHits(Ball ball)
        {
            if (!CanBallActiveHitsUpgrade(ball))
                return;

            var cost = GetBallActiveHitsUpgradeCost(ball);

            ball.Data.ActiveHitsLevel++;
            EventManager.I.TriggerBallUpgraded(ball, EStat.ActiveHits, cost);
        }

        #endregion

        #region Ball Active Damage

        private readonly float _ballActiveDamageBase = 1f;
        private readonly float _ballActiveDamageGrowthPerLevel = 0.1f;
        private readonly int _ballActiveDamageMaxLevel = 100;
        private readonly float _ballActiveDamageCostBase = 1.0f;
        private readonly float _ballActiveDamageCostGrowthPerLevel = 0.25f;

        public double GetBallActiveDamage(Ball ball, int level = -1)
        {
            if (level == -1)
                level = ball.Data.ActiveDamageLevel;

            return _ballActiveDamageBase + _ballActiveDamageGrowthPerLevel * (level - 1);
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

        public float GetBallActiveDamageUpgradeCost(Ball ball, int level = -1)
        {
            if (level == -1)
                level = ball.Data.ActiveDamageLevel;

            return _ballActiveDamageCostBase + _ballActiveDamageCostGrowthPerLevel * (level - 1);
        }

        public void UpgradeBallActiveDamage(Ball ball)
        {
            if (!CanBallActiveDamageUpgrade(ball))
                return;

            var cost = GetBallActiveDamageUpgradeCost(ball);

            ball.Data.ActiveDamageLevel++;
            EventManager.I.TriggerBallUpgraded(ball, EStat.ActiveDamage, cost);
        }

        #endregion

        #region Upgrade Points

        public float UpgradePointsCurrent = 0;
        private readonly float _upgradePointsBase = 2f;
        private readonly float _upgradePointsGrowthPerLevel = 0.075f;

        public float GetUpgradePointsForLevel(int level = -1)
        {
            if (level == -1)
                level = LevelCurrent - 1;

            return _upgradePointsBase + (level - 1) * _upgradePointsGrowthPerLevel;
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