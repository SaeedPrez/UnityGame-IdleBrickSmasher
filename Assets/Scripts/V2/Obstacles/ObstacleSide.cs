using Prez.V2.Enums;
using UnityEngine;

namespace Prez.V2.Obstacles
{
    public class ObstacleSide : MonoBehaviour
    {
        [field:SerializeField] public GameObject Obstacle { get; private set; }
        [field:SerializeField] public CollisionSide Side { get; private set; }
    }
}