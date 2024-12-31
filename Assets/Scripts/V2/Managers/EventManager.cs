using System;
using Prez.V2.Balls;
using Prez.V2.Obstacles;
using UnityEngine;

namespace Prez.V2.Managers
{
    public class EventManager : MonoBehaviour
    {
        #region Balls

        public static event Action<Ball, Brick> OnBallCollidedWithBrick = delegate { };
        public static void TriggerBallCollidedWithBrick(Ball ball, Brick brick) => OnBallCollidedWithBrick(ball, brick);

        #endregion
    }
}