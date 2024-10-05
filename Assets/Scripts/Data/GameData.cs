namespace Prez.Data
{
    public class GameData
    {
        // Bricks

        public float BrickNoiseScale = 4;
        public float BrickHealthIncreaseRate = 10f;

        public float BrickNoiseThresholdBase = 0.35f;
        public float BrickRowSpawnCooldownBase = 5f;
        public int BrickActiveBoostHitsBase = 1;

        public int BrickNoiseSeed = 0;
        public int BrickRowsSpawned = 0;
        
        
        // Balls

        public float BallSpeedBase = 3f;
        public NumberData BallDamageBase = new(1);
        
        
        // Coins

        public NumberData Coins = new(0);
        public NumberData Diamonds = new(0);
    }
}