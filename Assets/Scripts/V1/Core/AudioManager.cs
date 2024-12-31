using Prez.V1.Data;
using UnityEngine;

namespace Prez.V1.Core
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