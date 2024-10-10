using System.Collections.Generic;
using Prez.Utilities;

namespace Prez.Data
{
    public class GameData
    {
        #region Player

        public readonly float PlayerBaseSpeed = 2f;

        public float GetPlayerSpeed()
        {
            return PlayerBaseSpeed;
        }

        #endregion
        
        #region Bricks

        public readonly float BrickNoiseScale = 4;
        public readonly float BrickHealthIncreaseRate = 10f;

        public readonly float BrickNoiseThresholdBase = 0.25f;
        public readonly int BrickThresholdSpawnRowBase = 3;
        public readonly float BrickRowSpawnCooldownBase = 10f;
        public readonly int BrickActiveBoostHitsBase = 1;

        public int BrickNoiseSeed = 0;
        public double BrickNoiseOffsetY = 0;
        public double BrickRowsSpawned = 0d;

        public int GetBricksThresholdSpawnRow()
        {
            return BrickThresholdSpawnRowBase;
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
        public readonly float BallActivePlayDamageMultiplierBase = 1.5f;

        public float GetBallSpeed(Ball ball)
        {
            return BallSpeedBase;
        }

        public double GetBallDamage(Ball ball)
        {
            return BallDamageBase;
        }

        public float GetBallActivePlayBoostDamage(Ball ball)
        {
            return BallActivePlayDamageMultiplierBase;
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
        public readonly float ExperienceGainedPerHealthBase = 1.75f;
        public readonly float ExperienceLevelBase = 20f;
        public readonly float ExperienceLevelGrowth = 1.15f;

        public double GetExperienceGainedForHealth(double health)
        {
            return ExperienceGainedPerHealthBase * health;
        }

        public double GetExperienceNeededToLevel(double level)
        {
            return Helper.CalculateLevelCost(ExperienceLevelBase, ExperienceLevelGrowth, level);
        }

        #endregion
    }
}