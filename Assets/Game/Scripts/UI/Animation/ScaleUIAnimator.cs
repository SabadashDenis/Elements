using DG.Tweening;
using UnityEngine;

namespace Game.Scripts.Core
{
    [CreateAssetMenu(fileName = "ScaleUIAnimator", menuName = "Game/UIAnimator/ScaleUIAnimator")]
    public class ScaleUIAnimator : BaseUIAnimator
    {
        [SerializeField] private float targetScale = 1f;
        [SerializeField] private float duration = 0.5f;
        [SerializeField] private Ease ease = Ease.OutQuad;
        
        public override Tween Animate(View view)
        {
            var tween = view.RectTransform.DOScale(Vector3.one * targetScale, duration)
                .SetEase(ease)
                .SetUpdate(true);
            return tween;
        }

        public override void AnimateImmediately(View view)
        {
            view.RectTransform.localScale = Vector3.one * targetScale;
        }
    }
}