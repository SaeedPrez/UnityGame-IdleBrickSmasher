namespace Prez.Data
{
    public class BallData
    {
        public uint BallSpeedLevel = 1;
        public uint BallDamageLevel = 1;
        public uint BallCriticalChanceLevel = 1;
        public uint BallCriticalDamageLevel = 1;
        public readonly NumberData BallTotalDamage = new();
    }
}