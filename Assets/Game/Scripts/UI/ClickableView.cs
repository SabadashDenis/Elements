using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Scripts.Core
{
    public class ClickableView : View, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IClickableView
    {
        [SerializeField, FoldoutGroup("Clickable Settings")] protected BaseUIAnimator pointerDownAnimator;
        [SerializeField, FoldoutGroup("Clickable Settings")] protected BaseUIAnimator pointerUpAnimator;
        [SerializeField, FoldoutGroup("Clickable Settings")] protected View clickableContainer;
        [SerializeField, FoldoutGroup("Clickable Settings")] protected View viewContainer;
        
        protected Tween animTween;
        
        [ShowInInspector, FoldoutGroup("Debug", Order = 100), ReadOnly] private bool _isInteractable = true;
        private bool _asyncClickedFlag = false;
        
        public event Action OnDownEvent = delegate { };
        public event Action OnUpEvent = delegate { };
        public event Action OnClickEvent = delegate { };

        public bool IsInteractable
        {
            get => _isInteractable;
            set
            {
                if (clickableContainer != null)
                {
                    if (value)
                        clickableContainer.Show();
                    else
                        clickableContainer.Hide();
                }

                _isInteractable = value;
            }
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            OnUpEvent?.Invoke();
            OnUp(eventData);
            
            animTween?.Kill(true);
            if (pointerUpAnimator != null)
            {
                animTween = pointerUpAnimator.Animate(viewContainer);
            }
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            OnDownEvent?.Invoke();
            OnDown(eventData);
            
            animTween?.Kill(true);
            if (pointerDownAnimator != null)
            {
                animTween = pointerDownAnimator.Animate(viewContainer);
            }
        }
        
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (_isInteractable)
            {
                _asyncClickedFlag = true;
                
                OnClickEvent?.Invoke();
                OnClick(eventData);
            }
        }
        
        public async UniTask WaitForClick(CancellationToken token)
        {
            _asyncClickedFlag = false;

            await UniTask.WaitUntil(() => _asyncClickedFlag, cancellationToken: token);
        }
        
        public void RemoveAllOnDownEvents() => OnDownEvent = delegate { };

        public void RemoveAllOnUpEvents() => OnUpEvent = delegate { };

        public void RemoveAllOnClickEvents() => OnClickEvent = delegate { };


        protected virtual void OnDown(PointerEventData eventData) { }
        protected virtual void OnUp(PointerEventData eventData) { }
        protected virtual void OnClick(PointerEventData eventData) { }
    }
}