using UnityEngine;
using Random = UnityEngine.Random;

namespace Prez
{
    public class Ball : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.CompareTag("Brick"))
                return;
            
            var brick = other.gameObject.GetComponent<Brick>();
            brick.TakeDamage(1);
        }
    }
}
