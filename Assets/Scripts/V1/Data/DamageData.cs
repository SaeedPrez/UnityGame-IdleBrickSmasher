using UnityEngine;

namespace Prez.V1.Data
{
    public class DamageData
    {
        public Ball Ball;
        public Brick Brick;
        public bool BrickDestroyed;
        public bool ActiveHit;
        public bool CriticalHit;
        public double Damage;
        public double DamageRaw;
        public double Experience;
        public Vector3 Point;
    }
}