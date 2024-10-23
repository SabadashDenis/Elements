using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Scripts.Data;
using UnityEngine;

namespace Game.Scripts.Core
{
    public class SwapHandler : HandlerBase<LevelMapData, CancellationTokenSource>
    {
        [SerializeField] private SwapAnimData swapAnimConfig;

        public event Action OnSwapReleased = delegate { };

        protected override void OnBeforeHandle()
        {
            foreach (var levelElementData in Data.HandlerData.LevelElements)
            {
                levelElementData.ElementView.OnSwipe += (swipeDir) => OnBlockSwiped(swipeDir, levelElementData);
            }
        }

        private async void OnBlockSwiped(Direction swipeDirection, LevelElementData swipedElement)
        {
            if (CanBeSwiped(swipeDirection, swipedElement, out var swipeTarget))
            {
                await DoBlocksSwap(swipedElement.ElementView, swipeTarget.ElementView);
                OnSwapReleased.Invoke(); //CheckMatches();
            }
        }

        private bool CanBeSwiped(Direction swipeDirection, LevelElementData swipedElement,
            out LevelElementData swipeTarget)
        {
            var blockToSwapPos = swipedElement.ElementPos + DirectionUtility.GetOffset(swipeDirection);
            swipeTarget = Data.HandlerData.LevelElements.First((target) => target.ElementPos == blockToSwapPos);
            var swipeToJump = swipeDirection is Direction.Up && swipeTarget.ElementView.GetBlockType is BlockType.Empty;

            return !(swipedElement.ElementView.IsBusy || swipeTarget.ElementView.IsBusy) && !swipeToJump;
        }

        private async UniTask DoBlocksSwap(BlockView first, BlockView second)
        {
            var firstType = first.GetBlockType;
            var secondType = second.GetBlockType;

            var firstViewPos = first.View.position;
            var secondViewPos = second.View.position;

            var standardScale = first.View.localScale;

            first.SetBusy(true);
            second.SetBusy(true);

            bool swapCompleted = false;
            
            DOTween.Sequence()
                .Join(first.View.DOScale(standardScale * swapAnimConfig.ScaleInMultiplier,
                    swapAnimConfig.ScaleDuration))
                .Join(second.View
                    .DOScale(standardScale * swapAnimConfig.ScaleInMultiplier, swapAnimConfig.ScaleDuration)
                    .OnComplete(MoveToContainer))
                .Append(first.View.DOMove(secondViewPos, swapAnimConfig.MoveDuration)
                    .SetEase(Ease.InOutSine))
                .Join(second.View.DOMove(firstViewPos, swapAnimConfig.MoveDuration)
                    .SetEase(Ease.InOutSine)
                    .OnComplete(SwitchViews))
                .Append(first.View.DOScale(standardScale, swapAnimConfig.ScaleDuration))
                .Join(second.View.DOScale(standardScale, swapAnimConfig.ScaleDuration))
                .OnComplete(() => swapCompleted = true);


            void MoveToContainer()
            {
                first.View.SetParent(first.transform.parent.parent, true);
                second.View.SetParent(second.transform.parent.parent, true);
            }

            void SwitchViews()
            {
                first.View.transform.SetParent(first.transform, true);
                second.View.transform.SetParent(second.transform, true);

                first.SetType(secondType);
                second.SetType(firstType);

                first.View.anchorMin = second.View.anchorMin = Vector2.zero;
                first.View.anchorMax = second.View.anchorMax = Vector2.one;
                first.View.anchoredPosition = second.View.anchoredPosition = Vector2.zero;
            }
            
            await UniTask.WaitUntil(() => swapCompleted, cancellationToken: Data.TokenSource.Token);

            first.SetBusy(false);
            second.SetBusy(false);
        }
    }
}