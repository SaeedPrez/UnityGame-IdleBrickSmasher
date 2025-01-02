using Prez.V2.Data;
using UnityEngine;

namespace Prez.V2.Managers
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource _sfxAudioSource;
        [SerializeField] private float _pitchFrom;
        [SerializeField] private float _pitchTo;
        [SerializeField] private AudioClip[] _brickHitClips;
        [SerializeField] private AudioClip[] _brickDestroylips;
        
        private EventManager _event;

        #region Bootstrap

        private void Awake()
        {
            _event = RefManager.Event;
        }

        private void OnEnable()
        {
            _event.OnBrickDamaged += OnBrickDamaged;
        }

        private void OnDisable()
        {
            _event.OnBrickDamaged -= OnBrickDamaged;
        }

        #endregion

        #region Observers

        private void OnBrickDamaged(DamageData data)
        {
            if (data.BrickDestroyed)
                PlayBrickDestroyedSfx();
            else
                PlayBrickHitSfx();
        }

        #endregion

        #region Audio

        /// <summary>
        /// Plays a random brick hit audio.
        /// </summary>
        private void PlayBrickHitSfx()
        {
            _sfxAudioSource.pitch = Random.Range(_pitchFrom, _pitchTo);
            _sfxAudioSource.PlayOneShot(_brickHitClips[Random.Range(0, _brickHitClips.Length)]);
        }

        /// <summary>
        /// Plays a random brick destroyed audio.
        /// </summary>
        private void PlayBrickDestroyedSfx()
        {
            _sfxAudioSource.pitch = Random.Range(_pitchFrom, _pitchTo);
            _sfxAudioSource.PlayOneShot(_brickDestroylips[Random.Range(0, _brickDestroylips.Length)]);
        }
        
        #endregion
        
    }
}