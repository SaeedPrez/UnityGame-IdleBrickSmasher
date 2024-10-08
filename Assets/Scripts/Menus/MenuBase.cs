using UnityEngine;

namespace Prez.Menus
{
    public abstract class MenuBase : MonoBehaviour
    {
        [SerializeField] protected Vector2 PositionVisible;
        [SerializeField] protected Vector2 PositionHidden;

        protected RectTransform RectTransform;
        protected bool IsHiding;

        protected virtual void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
        }

        public abstract void Show();
        public abstract void Hide(bool skipAnimation = false);
    }
}