using System.Collections;
using DG.Tweening;
using Prez.V2.Data;
using Prez.V2.Managers;
using Prez.V2.Utilities;
using TMPro;
using UnityEngine;

namespace Prez.V2.Obstacles
{
    public class Brick : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _fillSprite;
        [SerializeField] private SpriteRenderer _borderSprite;
        [SerializeField] private TMP_Text _healthValueUi;
        [SerializeField] private Color _fillDamageColor;
        [SerializeField] private float _animationDuration;

        public bool IsActive { get; private set; }
        public double MaxHealth { get; private set; }
        public double Health { get; private set; }
        public Vector2Int GridPosition { get; private set; }
        public double RowId { get; private set; }
        public Color Color { get; private set; }
        
        private EventManager _event;
        private BoxCollider2D _collider;

        #region Bootstrap

        private void Awake()
        {
            _event = RefManager.Event;
            _collider = GetComponent<BoxCollider2D>();
            Color = _fillSprite.color;
        }

        private void OnEnable()
        {
            IsActive = true;
            _collider.enabled = true;
            _fillSprite.material.SetFloat(Constants.ShaderClip, 1f);
        }

        private void OnDisable()
        {
            StopAnimations();
        }

        #endregion
        
        #region Position

        /// <summary>
        /// Sets positions.
        /// </summary>
        /// <param name="gridPosition"></param>
        /// <param name="position"></param>
        public void SetPosition(Vector2Int gridPosition, Vector2 position)
        {
            GridPosition = gridPosition;
            transform.position = position;
        }
        
        /// <summary>
        /// Moves the brick down a row.
        /// </summary>
        /// <param name="position"></param>
        public void MoveDown(Vector2 position)
        {
            transform.position = new Vector3(transform.position.x, position.y, 0); 
            GridPosition += Vector2Int.up;
        }

        /// <summary>
        /// Set the row id where the brick spawned.
        /// </summary>
        /// <param name="rowId"></param>
        public void SetRowId(double rowId)
        {
            RowId = rowId;
        }
        
        #endregion

        #region Color

        /// <summary>
        /// Sets the brick color.
        /// </summary>
        /// <param name="color"></param>
        public void SetColor(Color color)
        {
            Color = color;
            _fillSprite.color = Color;
            _borderSprite.color = Color;
        }

        #endregion

        #region Health

        /// <summary>
        /// Sets max health.
        /// </summary>
        /// <param name="health"></param>
        public void SetMaxHealth(double health)
        {
            MaxHealth = health;
            Health = MaxHealth;
            UpdateHealthUi();
        }

        /// <summary>
        /// Takes damage.
        /// </summary>
        /// <param name="data"></param>
        public void TakeDamage(DamageData data)
        {
            if (!IsActive)
                return;
            
            var damage = data.Damage > Health
                ? Health
                : data.Damage;

            Health -= System.Math.Round(damage, 2);
            UpdateHealthUi();

            PlayDamageAnimation();

            if (Health <= 0.01d)
            {
                data.BrickDestroyed = true;
                Destroy();
            }

            _event.TriggerBrickDamaged(data);
        }

        /// <summary>
        /// Updates health Ui.
        /// </summary>
        private void UpdateHealthUi()
        {
            _healthValueUi.SetText(Helper.GetNumberAsString(Health));
        }

        #endregion

        #region Destroy

        /// <summary>
        /// Destroys brick.
        /// </summary>
        private void Destroy()
        {
            IsActive = false;
            _collider.enabled = false;

            StartCoroutine(DestroyAfterDelay());
        }

        private IEnumerator DestroyAfterDelay()
        {
            yield return new WaitForSeconds(_animationDuration + 0.01f);
            gameObject.SetActive(false);
        }

        #endregion

        #region Animations
        
        /// <summary>
        /// Plays damage animation.
        /// </summary>
        private void PlayDamageAnimation()
        {
            StopAnimations();
            AnimateFillColor();
            AnimateBorderColor();
            AnimateFillAmount();
        }

        /// <summary>
        /// Animates the fill color.
        /// </summary>
        private void AnimateFillColor()
        {
            _fillSprite.color = _fillDamageColor;
            _fillSprite.DOColor(Color, _animationDuration)
                .SetEase(Ease.InCirc);
        }

        /// <summary>
        /// Animates the border color.
        /// </summary>
        private void AnimateBorderColor()
        {
            _borderSprite.color = _fillDamageColor;
            _borderSprite.DOColor(Color, _animationDuration)
                .SetEase(Ease.InCirc);
        }
        
        /// <summary>
        /// Animates the fill amount.
        /// </summary>
        private void AnimateFillAmount()
        {
            _fillSprite.material.DOFloat((float)(Health / MaxHealth), Constants.ShaderClip, _animationDuration)
                .SetEase(Ease.OutCirc);
        }

        /// <summary>
        /// Stops all animations.
        /// </summary>
        private void StopAnimations()
        {
            _fillSprite.DOKill(true);
            _fillSprite.material.DOKill(true);
        }

        #endregion
    }
}