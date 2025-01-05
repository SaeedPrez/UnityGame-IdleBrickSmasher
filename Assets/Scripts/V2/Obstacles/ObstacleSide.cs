using Prez.V2.Enums;
using UnityEngine;

namespace Prez.V2.Obstacles
{
    public class ObstacleSide : MonoBehaviour
    {
        [field:SerializeField] public CollisionSide Side { get; private set; }
        [SerializeField] private Collider2D _collider;
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawCube(_collider.bounds.center, _collider.bounds.size);
        }
    }
}