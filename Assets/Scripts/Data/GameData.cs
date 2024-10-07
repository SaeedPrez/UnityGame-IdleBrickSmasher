using System.Collections.Generic;
using UnityEngine;

namespace Prez.Data
{
    public class GameData
    {
        // Bricks

        public readonly float BrickNoiseScale = 4;
        public readonly float BrickHealthIncreaseRate = 10f;

        public readonly float BrickNoiseThresholdBase = 0.35f;
        public readonly float BrickRowSpawnCooldownBase = 10f;
        public readonly int BrickActiveBoostHitsBase = 1;

        public int BrickNoiseSeed = 0;
        public uint BrickRowsSpawned = 0;
        
        
        // Balls

        public readonly List<BallData> Balls = new();
        public readonly float BallSpeedBase = 3f;
        public readonly NumberData BallDamageBase = new(1);
        public readonly float BallCriticalChanceBase = 0f;
        public readonly float BallCriticalDamageBase = 1f;
        
        
        // Coins

        public readonly NumberData Coins = new();
        public readonly int CoinsPerBrickBase = 1;

        public long GetCoins(long maxHealth)
        {
            return maxHealth * CoinsPerBrickBase;
        }
        
        
        // Diamonds
        
        public readonly NumberData Diamonds = new();
        
        
        // Experience & Level

        public uint Level = 1;
        public readonly NumberData ExperienceCurrent = new();
        public readonly NumberData ExperienceRequiredToLevel = new();
        public readonly int ExperiencePerHealthBase = 2;
        public readonly uint ExperienceLevelBase = 50;
        public readonly float ExperienceLevelGrowth = 1.1f;
        public readonly NumberData TimeTotal = new();
        public long TimeThisLevel = 0;

        public long GetExperience(long maxHealth)
        {
            return ExperiencePerHealthBase * maxHealth;
        }
        
        public long GetExperienceNeededToLevel(uint level)
        {
            return (long)(ExperienceLevelBase * Mathf.Pow(ExperienceLevelGrowth, level));
        }
    }
}