namespace Prez.V2.Data.Save
{
    public class BricksData
    {
        // The noise map scale.
        public readonly float NoiseScale = 4.0f;

        // The noise map seed.
        public int NoiseSeed = 0;

        // The noise map Y-axis offset.
        public long NoiseOffsetY = 0;

        // The number of spawned rows
        // that had bricks.
        public long RowsSpawned = 0;

        // Base chance of a brick spawning (0.0f to 1.0f)
        // based on the noise value.
        public readonly float NoiseSpawnThresholdBase = 0.3f;
        
        // Base number of spawned bricks. 
        public readonly int MinimumSpawnBase = 10;

        // Base cooldown until a new row of bricks spawns. 
        public readonly float RowSpawnCooldownBase = 11f;

        public readonly float HealthBase = 1f;
        public readonly float HealthGrowthPerLevel = 0.25f;
        
        public float GetNoiseSpawnThreshold()
        {
            return NoiseSpawnThresholdBase;
        }

        public int GetMinimumBricksThreshold()
        {
            return MinimumSpawnBase;
        }

        public float GetSpawnCooldown()
        {
            return RowSpawnCooldownBase;
        }

        public double GetMaxHealth(long level = -1)
        {
            if (level == -1)
                level = RowsSpawned;

            return HealthBase + HealthGrowthPerLevel * level;
        }
    }
}