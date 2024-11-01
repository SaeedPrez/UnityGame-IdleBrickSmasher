using System;
using Prez.Data;
using Prez.Enums;
using UnityEngine;

namespace Prez.Core
{
    public class PlayerBullet : MonoBehaviour
    {
        [SerializeField] private float _speed;
        
        private Rigidbody2D _rigidbody;
        
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            if (GameManager.State is EGameState.Playing)
                Move();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag(Constants.Brick))
            {
                var brick = other.gameObject.GetComponent<Brick>();
                EventManager.I.TriggerBulletCollidedWithBrick(brick, other.contacts[0].point);
                DamageBrick(brick, other.contacts[0].point);
            }
                
            gameObject.SetActive(false);
        }
        
        private void Move()
        {
            _rigidbody.MovePosition(_rigidbody.position + (_speed * Time.fixedDeltaTime * Vector2.up));
        }
        
        /// <summary>
        ///     Damages brick and reduces balls active player boost.
        /// </summary>
        /// <param name="brick"></param>
        /// <param name="point"></param>
        private void DamageBrick(Brick brick, Vector3 point)
        {
            var damage = 10d;

            var data = new DamageData
            {
                Brick = brick,
                Point = point,
                DamageRaw = damage,
                CriticalHit = false
            };

            brick.TakeDamage(data);
        }
    }
}