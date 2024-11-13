﻿using Prez.Data;
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

        private void OnEnable()
        {
            _rigidbody.linearVelocityY = GameManager.Data.GetPlayerBulletSpeed();
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
        
        /// <summary>
        ///     Damages brick and reduces balls active player boost.
        /// </summary>
        /// <param name="brick"></param>
        /// <param name="point"></param>
        private void DamageBrick(Brick brick, Vector3 point)
        {
            var data = new DamageData
            {
                Brick = brick,
                Point = point,
                DamageRaw = GameManager.Data.GetPlayerBulletDamage(),
                CriticalHit = false
            };

            brick.TakeDamage(data);
        }
    }
}