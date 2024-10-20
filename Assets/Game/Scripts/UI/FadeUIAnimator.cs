using DG.Tweening;
using UnityEngine;

namespace Game.Scripts.Core
{
    [CreateAssetMenu(fileName = "FadeUIAnimator", menuName = "Game/UIAnimator")]
    public class FadeUIAnimator : BaseUIAnimator
    {
        [SerializeField] private float delay = 0f;
        [SerializeField] private float duration = 4f;
        [SerializeField] private float opacity = 1f;
        [SerializeField] private Ease ease = Ease.OutQuad;
        
        public override Tween Animate(View view)
        {
            var tween = view.CanvasGroup.DOFade(opacity, duration)
                .SetDelay(delay)
                .SetEase(ease)
                .SetUpdate(true);
            return tween;
        }

        public override void AnimateImmediately(View view)
        {
            view.CanvasGroup.alpha = opacity;
        }
    }
}