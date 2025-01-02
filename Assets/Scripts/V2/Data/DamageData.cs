using Prez.V2.Balls;
using Prez.V2.Obstacles;
using UnityEngine;

namespace Prez.V2.Data
{
    public class DamageData
    {
        public Ball Ball;
        public Brick Brick;

        // The base damage.
        public double DamageBase;
        
        // The calculated damage.
        public double Damage;

        // IF hit was critical.
        public bool CriticalHit;

        // If hit was active.
        public bool ActiveHit;

        // Active hit boost.
        public float ActiveHitBoost;
        
        // If brick was destroyed by this damage.
        public bool BrickDestroyed;

        // The Collision point.
        public Vector3 CollisionPoint;

        // The collision normal.
        public Vector3 CollisionNormal;

        // Experience gained.
        public double Experience;
    }
}