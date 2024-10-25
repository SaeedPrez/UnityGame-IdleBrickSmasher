using UnityEngine;

namespace Core
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource _sfxPlayer;
        [SerializeField] private AudioClip[] _brickHitClips;

        private void OnEnable()
        {
            EventManager.I.OnBrickDamaged += OnBrickDamaged;
        }

        private void OnDisable()
        {
            EventManager.I.OnBrickDamaged -= OnBrickDamaged;
        }
        
        private void OnBrickDamaged(Brick brick, Ball ball, double damage, bool activeBoost, bool critical, bool destroyed)
        {
            _sfxPlayer.PlayOneShot(_brickHitClips[0]);
        }

    }
}