using DG.Tweening;
using UnityEngine;

namespace Prez.Menus
{
    public class BallsMenu : MenuBase
    {
        public override void Show()
        {
            gameObject.SetActive(true);
            RectTransform.DOKill();
            RectTransform.DOAnchorPos(PositionVisible, 0.25f)
                .SetEase(Ease.OutCirc);
        }

        public override void Hide(bool skipAnimation = false)
        {
            if (IsHiding) 
                return;

            var duration = 0.25f;
            
            IsHiding = true;
            RectTransform.DOKill();
            RectTransform.DOAnchorPos(PositionHidden, duration)
                .SetEase(Ease.InCirc)
                .OnComplete(() =>
                {
                    IsHiding = false;
                    gameObject.SetActive(false);
                });

            if (skipAnimation)
                RectTransform.DOKill(true);
        }
    }
}