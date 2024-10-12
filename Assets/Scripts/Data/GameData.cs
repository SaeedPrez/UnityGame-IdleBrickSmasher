using System.Collections.Generic;
using Utilities;

namespace Data
{
    public class GameData
    {
        #region Player

        private readonly float _playerSpeedBase = 2f;

        public float GetPlayerSpeed()
        {
            return _playerSpeedBase;
        }

        #endregion
        
        #region Bricks

        public readonly float BrickNoiseScale = 4;
        public readonly float BrickHealthIncreaseRate = 10f;

        private readonly float _brickNoiseThresholdBase = 0.35f;
        private readonly int _brickThresholdSpawnRowBase = 3;
        private readonly float _brickRowSpawnCooldownBase = 10f;

        public int BrickNoiseSeed = 0;
        public double BrickNoiseOffsetY = 0;
        public double BrickRowsSpawned = 0;

        public float GetBrickNoiseThreshold()
        {
            return _brickNoiseThresholdBase;
        }

        public float GetBrickSpawnCooldown()
        {
            return _brickRowSpawnCooldownBase;
        }
        
        public int GetBrickThresholdSpawnRow()
        {
            return _brickThresholdSpawnRowBase;
        }

        #endregion

        #region Balls

        public readonly List<BallData> Balls = new()
        {
            new BallData { UnlockLevel = 1 },
            new BallData { UnlockLevel = 2 },
            new BallData { UnlockLevel = 4 },
            new BallData { UnlockLevel = 8 },
            new BallData { UnlockLevel = 12 },
            new BallData { UnlockLevel = 16 },
            new BallData { UnlockLevel = 20 },
            new BallData { UnlockLevel = 25 },
            new BallData { UnlockLevel = 30 },
            new BallData { UnlockLevel = 40 },
            new BallData { UnlockLevel = 50 },
            new BallData { UnlockLevel = 75 },
            new BallData { UnlockLevel = 100 },
        };

        public readonly float BallSpeedBase = 2.5f;
        public readonly float BallDamageBase = 1f;
        public readonly float BallCriticalChanceBase = 0f;
        public readonly float BallCriticalDamageBase = 1f;

        public float GetBallSpeed(Ball ball)
        {
            return BallSpeedBase;
        }

        public double GetBallDamage(Ball ball)
        {
            return BallDamageBase;
        }

        #endregion

        #region Active Play

        private readonly int _activePlayHitsBase = 1;
        private readonly float _activePlayDmgMultiplierBase = 1.5f;
        private readonly float _activePlayExpMultiplierBase = 1.5f;

        public int GetActivePlayHits(Ball ball)
        {
            return _activePlayHitsBase;
        }

        public float GetActivePlayDamageMultiplier(Ball ball)
        {
            return _activePlayDmgMultiplierBase;
        }
        
        public float GetActivePlayExpMultiplier(Ball ball)
        {
            return _activePlayExpMultiplierBase;
        }
        
        #endregion
        
        #region Coins

        public double Coins = 0d;
        public readonly float CoinsPerBrickBase = 0.25f;
        public readonly float CoinsPerLevelBase = 2.5f;

        public double GetCoinsGainedPerHealth(double maxHealth)
        {
            return maxHealth * CoinsPerBrickBase;
        }

        public double GetCoinsGainedPerLevel(double level)
        {
            return CoinsPerLevelBase * level;
        }

        #endregion

        #region Diamonds

        public double Diamonds = 0d;
        public readonly float DiamondsPerLevelBase = 0.25f;

        public double GetDiamondsGainedPerLevel(double level)
        {
            return DiamondsPerLevelBase * level;
        }

        #endregion

        #region Level & Time

        public int Level = 1;
        public double TimeThisLevel = 0d;
        public double TimeTotal = 0d;
        public double ExperienceCurrent = 0d;
        public double ExperienceRequiredToLevel = 0d;
        private readonly float _expPerDamageBase = 0.67f;
        private readonly float _expPerHealthBase = 0.5f;
        private readonly float _expLevelBase = 20f;
        private readonly float _expLevelGrowth = 1.15f;

        public double GetExpForDamage(double damage)
        {
            return _expPerDamageBase * damage;
        }

        public double GetExpForDestroyed(double health)
        {
            return _expPerHealthBase * health;
        }

        public double GetExpNeededToLevel(double level)
        {
            return Helper.CalculateLevelCost(_expLevelBase, _expLevelGrowth, level);
        }

        #endregion
    }
}