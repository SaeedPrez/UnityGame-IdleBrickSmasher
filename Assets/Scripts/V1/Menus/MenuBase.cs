﻿using DG.Tweening;
using Prez.V1.Core;
using UnityEngine;

namespace Prez.V1.Menus
{
    public class MenuBase : MonoBehaviour
    {
        [SerializeField] protected Vector2 PositionVisible;
        [SerializeField] protected Vector2 PositionHidden;
        protected bool IsHiding;

        protected RectTransform RectTransform;

        protected virtual void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
        }

        public virtual void Show()
        {
            if (gameObject.activeInHierarchy)
                return;

            gameObject.SetActive(true);
            RectTransform.DOKill();
            RectTransform.DOAnchorPos(PositionVisible, 0.25f)
                .SetEase(Ease.OutCirc);
        }

        public virtual void Hide(bool skipAnimation = false)
        {
            if (!gameObject.activeInHierarchy)
                return;

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

            EventManager.I.TriggerBottomMenuHidden(this);
        }
    }
}