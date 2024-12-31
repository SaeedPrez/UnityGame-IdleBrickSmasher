using System;
using Prez.V2.Balls;
using Prez.V2.Managers;
using UnityEngine;

namespace Prez.V2.Obstacles
{
    public class Brick : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _fillSprite;
        
        private int _health = 5;
        
        private void OnEnable()
        {
            EventManager.OnBallCollidedWithBrick += OnBallCollidedWithBrick;            
        }

        private void OnDisable()
        {
            EventManager.OnBallCollidedWithBrick -= OnBallCollidedWithBrick;            
        }

        #region Observers

        private void OnBallCollidedWithBrick(Ball ball, Brick brick)
        {
            if (this != brick)
                return;
            
            _health--;

            _fillSprite.material.SetFloat("_Clip", _health / 5f);
            
            if (_health == 0)
                Destroy(gameObject);
        }

        #endregion
    }
}