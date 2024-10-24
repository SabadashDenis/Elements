using System;
using UnityEngine;

namespace Game.Scripts.Data
{
    [Serializable]
    public struct FallAnimData
    {
        [SerializeField] private float scaleInMultiplier;
        [SerializeField] private float scaleDuration;
        [SerializeField] private float moveDuration;

        public float ScaleInMultiplier => scaleInMultiplier;
        public float ScaleDuration => scaleDuration;
        public float MoveDuration => moveDuration;
    }
}