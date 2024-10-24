using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Scripts.Data;
using UnityEngine;

namespace Game.Scripts.Core
{
    public class FallBlocksHandler : HandlerBase<LevelMapData, CancellationTokenSource>
    {
        public void StartFallRoutine()
        {
            FallAllBlocksRoutine();
        }

        private void FallAllBlocksRoutine()
        {
            Dictionary<Vector2Int, int> fallTargets = new();

            for (int y = 0; y < Data.HandlerData.MapSize.y; y++)
            {
                for (int x = 0; x < Data.HandlerData.MapSize.x; x++)
                {
                    var elementPos = new Vector2Int(x, y);

                    if (CanFall(elementPos, out var cellsToFall))
                    {
                        fallTargets.Add(elementPos, cellsToFall);
                    }
                }

                if (fallTargets.Count == Data.HandlerData.MapSize.x)
                    break;
            }

            foreach (var fallTarget in fallTargets)
            {
                FallOneColumn(fallTarget.Key, fallTarget.Value);
            }
        }

        private bool CanFall(Vector2Int elementPos, out int cellsToFall)
        {
            bool canFall = false;
            cellsToFall = 0;

            if (Data.HandlerData.LevelElements.TryGetValue(elementPos, out var elementView))
            {
                if (elementView.GetBlockType is BlockType.Empty)
                    return false;
                
                for (int i = 0; i < elementPos.y; i++)
                {
                    var bottomElementPos = elementPos + DirectionUtility.GetOffset(Direction.Down) * (i + 1);
                    
                    if (Data.HandlerData.LevelElements.TryGetValue(bottomElementPos, out var bottomElementView))
                    {
                        if (bottomElementView.GetBlockType is BlockType.Empty)
                        {
                            cellsToFall++;
                            canFall = true;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            
            return canFall;
        }

        private void FallOneColumn(Vector2Int firstFallenElementPos, int cellsToFall)
        {
            var fallingColumn = CollectFallingColumn(firstFallenElementPos);

            foreach (var fallingElementPos in fallingColumn)
            {
                DoFall(fallingElementPos, cellsToFall).Forget();
            }
        }

        private List<Vector2Int> CollectFallingColumn(Vector2Int firstFallenElementPos)
        {
            List<Vector2Int> fallingColumn = new();
            var maxY = Data.HandlerData.MapSize.y;

            for (int yPos = firstFallenElementPos.y; yPos < maxY; yPos++)
            {
                var checkedPos = new Vector2Int(firstFallenElementPos.x, yPos);

                if (Data.HandlerData.LevelElements.TryGetValue(checkedPos, out var elementView))
                {
                    if (elementView.GetBlockType is not BlockType.Empty)
                    {
                        fallingColumn.Add(checkedPos);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return fallingColumn;
        }

        private async UniTask DoFall(Vector2Int fallingElementPos, int cellsToFall)
        {
            var targetPos = fallingElementPos + DirectionUtility.GetOffset(Direction.Down) * cellsToFall;

            if (Data.HandlerData.LevelElements.TryGetValue(fallingElementPos, out var fallingBlockView))
            {
                if (Data.HandlerData.LevelElements.TryGetValue(targetPos, out var targetBlockView))
                {
                    bool fallCompleted = false;
                    var standardViewScale = fallingBlockView.View.localScale;
                    
                    fallingBlockView.SetBusy(true);
                    targetBlockView.SetBusy(true);
                    
                    DOTween.Sequence()
                        .Join(fallingBlockView.View.DOScale(standardViewScale * 0.75f,
                            0.2f))
                        .Join(fallingBlockView.View.DOMove(targetBlockView.View.position, 0.25f * cellsToFall)
                            .SetEase(Ease.InOutSine))
                        .Append(fallingBlockView.View.DOScale(standardViewScale, 0.2f))
                        .OnComplete(() =>
                        {
                            SwitchViews();
                            fallCompleted = true;
                        });
                    
                    void SwitchViews()
                    {
                        targetBlockView.SetType(fallingBlockView.GetBlockType);
                        fallingBlockView.SetType(BlockType.Empty);
                        
                        targetBlockView.View.anchorMin = fallingBlockView.View.anchorMin = Vector2.zero;
                        targetBlockView.View.anchorMax = fallingBlockView.View.anchorMax = Vector2.one;
                        targetBlockView.View.anchoredPosition = fallingBlockView.View.anchoredPosition = Vector2.zero;
                    }

                    await UniTask.WaitUntil(() => fallCompleted, cancellationToken: Data.TokenSource.Token);
                    
                    fallingBlockView.SetBusy(false);
                    targetBlockView.SetBusy(false);
                }
            }
        }
    }
}