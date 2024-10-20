using DG.Tweening;
using UnityEngine;

namespace Game.Scripts.Core
{
    public abstract class BaseUIAnimator : ScriptableObject
    {
        public abstract Tween Animate(View view);
        public abstract void AnimateImmediately(View view);
    }
}