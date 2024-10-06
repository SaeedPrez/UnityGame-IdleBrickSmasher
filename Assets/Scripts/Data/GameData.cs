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

        public readonly float BallSpeedBase = 3f;
        public readonly NumberData BallDamageBase = new(1);
        
        
        // Coins

        public readonly NumberData Coins = new();
        public readonly int CoinsPerBrickBase = 1;
        
        
        // Diamonds
        
        public readonly NumberData Diamonds = new();
        
        
        // Experience & Level

        public uint Level = 0;
        public readonly NumberData ExperienceCurrent = new();
        public readonly NumberData ExperienceRequiredToLevel = new();
        public readonly int ExperiencePerHealthBase = 2;
        public readonly uint ExperienceLevelBase = 50;
        public readonly float ExperienceLevelGrowth = 1.1f;
    }
}