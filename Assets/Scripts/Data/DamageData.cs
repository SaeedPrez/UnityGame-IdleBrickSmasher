using UnityEngine;

namespace Prez.Data
{
    public class DamageData
    {
        public Brick Brick;
        public Ball Ball;
        public Vector3 Point;
        public double Damage;
        public double DamageRaw;
        public bool CriticalHit;
        public bool ActiveHit;
        public bool BrickDestroyed;
    }
}