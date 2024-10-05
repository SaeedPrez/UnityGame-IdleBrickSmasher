namespace Prez.Data
{
    public class GameData
    {
        // Ball
        public float BallSpeedBase = 5f;
        public int BallDamageBase = 1;
        
        // Bricks
        public int BrickRowsSpawned = 0;
        public int BrickNoiseSeed = 0;
        public float BrickNoiseScale = 4;
        public float BrickNoiseThreshold = 0.45f;
        public float BrickHealthIncreaseRate = 10f;
        public float BrickRowSpawnCooldown = 5f;
    }
}