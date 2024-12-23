using System;
using System.Collections;
using Prez.Core;
using Prez.Data;
using Prez.Utilities;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Prez
{
    [SelectionBase]
    public class Ball : MonoBehaviour
    {
        [SerializeField] private TrailRenderer _trail;
        [SerializeField] private TMP_Text _ballIdUi;
        [SerializeField] private float _dpsDuration;
        private int _activePlayBoostHits;
        private Coroutine _ballDpsCoroutine;
        private Coroutine _ballVelocityCoroutine;
        private float _lastHitAt;

        private Rigidbody2D _rigidbody;

        public bool IsPlayerBoostActive { get; private set; }
        public BallData Data { get; private set; }
        public double Dps { get; private set; }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _trail.emitting = false;
        }

        private void OnEnable()
        {
            _ballVelocityCoroutine = StartCoroutine(RespawnBallIfStuck());
            _lastHitAt = Time.time;
        }

        private void OnDisable()
        {
            StopCoroutine(_ballVelocityCoroutine);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag(Constants.Player))
            {
                EventManager.I.TriggerBallCollidedWithPlayer(this);
                EnableActivePlayBoost();
                _lastHitAt = Time.time;
            }
            else if (other.gameObject.CompareTag(Constants.WallBottom))
            {
                EventManager.I.TriggerBallCollidedWithBottomWall(this);
                DisableActivePlayBoost();
            }
            else if (other.gameObject.CompareTag(Constants.Brick))
            {
                var brick = other.gameObject.GetComponent<Brick>();
                EventManager.I.TriggerBallCollidedWithBrick(this, brick, other.contacts[0].point);
                DamageBrick(brick, other.contacts[0].point);
                _lastHitAt = Time.time;
            }
        }

        #region Data

        public void SetData(BallData data)
        {
            Data = data;
            _ballIdUi.SetText(Data.Id.ToString());
        }

        #endregion

        #region Brick

        /// <summary>
        ///     Damages brick and reduces balls active player boost.
        /// </summary>
        /// <param name="brick"></param>
        /// <param name="point"></param>
        private void DamageBrick(Brick brick, Vector3 point)
        {
            var damage = GameManager.Data.GetBallDamage(this);

            if (IsPlayerBoostActive)
                damage *= GameManager.Data.GetBallActiveDamage(this);

            var data = new DamageData
            {
                Brick = brick,
                Ball = this,
                Point = point,
                DamageRaw = damage,
                ActiveHit = IsPlayerBoostActive,
                CriticalHit = Helper.IsHitCritical(GameManager.Data.GetBallCriticalChance(this)),
            };

            data = Helper.CalculateDamage(data);

            brick.TakeDamage(data);
            ReduceActivePlayBoostHits();
            Data.TotalDamage += data.Damage;
        }

        #endregion

        #region Velocity

        /// <summary>
        ///     Sets a random direction velocity.
        /// </summary>
        public void SetRandomDirectionVelocity()
        {
            _rigidbody.linearVelocity = new Vector3(Random.Range(-2f, 2f), 1, 0).normalized * GameManager.Data.GetBallSpeed(this);
        }

        /// <summary>
        ///     Changes ball X velocity.
        /// </summary>
        /// <param name="value"></param>
        public void ChangeVelocityX(float value)
        {
            StartCoroutine(ChangeVelocityXAfterDelay(value));
        }

        /// <summary>
        ///     Changes ball X velocity after a delay.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        private IEnumerator ChangeVelocityXAfterDelay(float value, float delay = 0.01f)
        {
            yield return new WaitForSeconds(delay);

            // var speed = _rigidbody.linearVelocity.magnitude;
            _rigidbody.linearVelocity = new Vector2(value, 1).normalized * GameManager.Data.GetBallSpeed(this);
        }

        /// <summary>
        ///     Checks and respawns the ball if stuck.
        /// </summary>
        /// <returns></returns>
        private IEnumerator RespawnBallIfStuck()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);

                var expectedVelocity = GameManager.Data.GetBallSpeed(this);
                var currentVelocity = _rigidbody.linearVelocity.magnitude;

                if (_lastHitAt < Time.time - 20f)
                {
                    EventManager.I.TriggerBallRequestRespawn(this);
                    _lastHitAt = Time.time;
                }
                else if (currentVelocity <= expectedVelocity * 0.5f)
                {
                    EventManager.I.TriggerBallRequestRespawn(this);
                }
                else if (Math.Abs(currentVelocity - expectedVelocity) > 0.01f)
                {
                    _rigidbody.linearVelocity = _rigidbody.linearVelocity.normalized * expectedVelocity;
                }
            }
        }

        #endregion

        #region Active Player Boost

        /// <summary>
        ///     Activates the active play boost.
        /// </summary>
        private void EnableActivePlayBoost()
        {
            IsPlayerBoostActive = true;
            _trail.emitting = true;
            _activePlayBoostHits = GameManager.Data.GetBallActiveHits(this);
        }

        /// <summary>
        ///     Reduces active play boost hits.
        /// </summary>
        private void ReduceActivePlayBoostHits()
        {
            if (_activePlayBoostHits <= 0)
                return;

            _activePlayBoostHits--;

            if (_activePlayBoostHits == 0)
                DisableActivePlayBoost();
        }

        /// <summary>
        ///     Disables the active player boost.
        /// </summary>
        private void DisableActivePlayBoost()
        {
            IsPlayerBoostActive = false;
            _trail.emitting = false;
            _activePlayBoostHits = 0;
        }

        #endregion
    }
}