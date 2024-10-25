using Prez.Data;
using UnityEngine;

namespace Prez.Core
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
        
        private void OnBrickDamaged(DamageData data)
        {
            _sfxPlayer.PlayOneShot(_brickHitClips[0]);
        }

    }
}