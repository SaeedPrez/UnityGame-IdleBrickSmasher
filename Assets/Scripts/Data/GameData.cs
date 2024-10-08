using System.Collections.Generic;
using Prez.Utilities;

namespace Prez.Data
{
    public class GameData
    {
        #region Bricks

        public readonly float BrickNoiseScale = 4;
        public readonly float BrickHealthIncreaseRate = 10f;

        public readonly float BrickNoiseThresholdBase = 0.35f;
        public readonly float BrickRowSpawnCooldownBase = 10f;
        public readonly int BrickActiveBoostHitsBase = 1;

        public int BrickNoiseSeed = 0;
        public double BrickRowsSpawned = 0d;

        #endregion
        
        #region Balls

        public readonly List<BallData> Balls = new();
        public readonly float BallSpeedBase = 3f;
        public readonly float BallDamageBase = 1f;
        public readonly float BallCriticalChanceBase = 0f;
        public readonly float BallCriticalDamageBase = 1f;

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
        public readonly float DiamondsPerLevelBase = 0.5f;
        
        public double GetDiamondsGainedPerLevel(double level)
        {
            return DiamondsPerLevelBase * level;
        }

        #endregion

        #region Level & Time

        public double Level = 1d;
        public double TimeThisLevel = 0d;
        public double TimeTotal = 0d;
        public double ExperienceCurrent = 0d;
        public double ExperienceRequiredToLevel = 0d;
        public readonly float ExperienceGainedPerHealthBase = 2f;
        public readonly float ExperienceLevelBase = 50f;
        public readonly float ExperienceLevelGrowth = 1.15f;

        public double GetExperienceGainedPerHealth(double maxHealth)
        {
            return ExperienceGainedPerHealthBase * maxHealth;
        }
        
        public double GetExperienceNeededToLevel(double level)
        {
            return Helper.CalculateLevelCost(ExperienceLevelBase, ExperienceLevelGrowth, level);
        }

        #endregion
    }
}