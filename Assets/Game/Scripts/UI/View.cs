using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Core
{
    [RequireComponent(typeof(CanvasGroup)), RequireComponent(typeof(RectTransform))]
    public class View : MonoBehaviour, IView, IInitable
    {
        [SerializeField, FoldoutGroup("Default References", Order = -100)] private CanvasGroup canvasGroup;
        [SerializeField, FoldoutGroup("Default References", Order = -100)] private RectTransform rectTransform;
        
        [SerializeField, FoldoutGroup("View Settings")] protected BaseUIAnimator showAnimation;
        [SerializeField, FoldoutGroup("View Settings")] protected BaseUIAnimator hideAnimation;
        
        [FoldoutGroup("Debug", Order = 100), ShowInInspector, ReadOnly] private bool _isVisible;
        [FoldoutGroup("Debug", Order = 100), ShowInInspector, ReadOnly] private bool _isInitialized;
        
        private Tween _currentTween;

        public CanvasGroup CanvasGroup => canvasGroup ? canvasGroup : canvasGroup = GetComponent<CanvasGroup>();

        public RectTransform RectTransform => rectTransform ? rectTransform : rectTransform = GetComponent<RectTransform>();
        
        [FoldoutGroup("Debug"), Button]
        public void Init()
        {
            if (!_isInitialized)
            {
                _isInitialized = true;
                OnInit();
            }
        }

        protected virtual void OnInit() { }

        [FoldoutGroup("Debug"), Button]
        public Tween Show()
        {
            if (showAnimation != null)
            {
                if (!_isVisible)
                {
                    _currentTween?.Kill();
                    _currentTween = showAnimation.Animate(this);

                    gameObject.SetActive(true);
                    OnShowStart();
                    
                    return _currentTween.OnComplete(() =>
                    {
                        _isVisible = true;
                        OnShowEnd();
                    });
                }
            }
            else
            {
                ShowImmediately();
            }
            
            return DOTween.Sequence();
        }

        [FoldoutGroup("Debug"), Button]
        public Tween Hide()
        {
            if (hideAnimation != null)
            {
                if (_isVisible)
                {
                    _isVisible = false;

                    _currentTween?.Kill();
                    _currentTween = hideAnimation.Animate(this);

                    OnHideStart();
                    
                    return _currentTween.OnComplete(() =>
                    {
                        OnHideEnd();
                        gameObject.SetActive(false);
                    });
                }
            }
            else
            {
                HideImmediately();
            }

            return DOTween.Sequence();
        }


        [FoldoutGroup("Debug"), Button]
        public void ShowImmediately()
        { 
            showAnimation?.AnimateImmediately(this);
            gameObject.SetActive(true);
            _isVisible = true;
            
            OnShowStart();
            OnShowEnd();
        }

        [FoldoutGroup("Debug"), Button]
        public void HideImmediately()
        { 
            hideAnimation?.AnimateImmediately(this);
            gameObject.SetActive(false);
            _isVisible = false;
            
            OnHideStart();
            OnHideEnd();
        }
        
        
        protected virtual void OnShowStart(){}
        protected virtual void OnShowEnd(){}
        protected virtual void OnHideStart(){}
        protected virtual void OnHideEnd(){}
    }
}