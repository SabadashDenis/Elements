using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Data;
using UnityEngine;

namespace Game.Scripts.Core
{
    public class CheckMatchHandler : HandlerBase<LevelMapData, CancellationTokenSource>
    {
        public event Action OnMatchFound = delegate {  };

        public void StartCheckMatchRoutine()
        {
            CheckMatches().Forget();
        }
        
        private async UniTask CheckMatches()
        {
            if (TryFindMatch(out var elementWithMatch))
            {
                List<Vector2Int> fullMatchGroup = new();
                CollectFullMatchGroup(elementWithMatch, ref fullMatchGroup);

                List<UniTask> destroyTasks = new();
                
                foreach (var matchedElementPos in fullMatchGroup)
                {
                    if (Data.HandlerData.LevelElements.TryGetValue(matchedElementPos, out var blockView))
                    {
                        destroyTasks.Add(blockView.Destroy(Data.TokenSource.Token));
                    }
                }

                await UniTask.WaitWhile(() => destroyTasks.Any((task) => !task.GetAwaiter().IsCompleted),
                    cancellationToken: Data.TokenSource.Token);
                
                OnMatchFound.Invoke();
            }
        }
        
        private void CollectFullMatchGroup(Vector2Int checkedPos, ref List<Vector2Int> collectList)
        {
            if (Data.HandlerData.LevelElements.TryGetValue(checkedPos, out var checkedElement))
            {
                if (checkedElement.GetBlockType is BlockType.Empty || checkedElement.IsBusy)
                    return;

                foreach (var direction in DirectionUtility.AllDirections)
                {
                    var neighborPos = checkedPos + DirectionUtility.GetOffset(direction);

                    if (Data.HandlerData.LevelElements.TryGetValue(neighborPos, out var neighborElement))
                    {
                        var hasSameTypes = neighborElement.GetBlockType == checkedElement.GetBlockType;
                        var notCollected = !collectList.Contains(neighborPos);

                        if (hasSameTypes && notCollected && !neighborElement.IsBusy)
                        {
                            collectList.Add(neighborPos);
                            CollectFullMatchGroup(neighborPos, ref collectList);
                        }
                    }
                }
            }
        }
        
        private bool TryFindMatch(out Vector2Int elementWithMatch)
        {
            elementWithMatch = new Vector2Int();
            
            foreach (var levelElement in Data.HandlerData.LevelElements)
            {
                if (HasThreeInRow(levelElement.Key))
                {
                    elementWithMatch = levelElement.Key;
                    return true;
                }
            }

            return false;
        }

        private bool HasThreeInRow(Vector2Int elementPos)
        {
            var verticalMatch = HasMatchInDirections(elementPos, DirectionUtility.VerticalDirections);
            var horizontalMatch = HasMatchInDirections(elementPos, DirectionUtility.HorizontalDirections);

            return (horizontalMatch || verticalMatch);
        }
        
        private bool HasMatchInDirections(Vector2Int checkedElement, List<Direction> directions)
        {
            if (Data.HandlerData.LevelElements.TryGetValue(checkedElement, out var checkedBlockView))
            {
                if (checkedBlockView.GetBlockType is BlockType.Empty || checkedBlockView.IsBusy)
                    return false;
                
                foreach (var direction in directions)
                {
                    var neighborPos = checkedElement + DirectionUtility.GetOffset(direction);
                    
                    if (Data.HandlerData.LevelElements.TryGetValue(neighborPos, out var neighborBlockView))
                    {
                        var anyIsBusy = checkedBlockView.IsBusy || neighborBlockView.IsBusy;
                        var hasDifferentTypes = neighborBlockView.GetBlockType != checkedBlockView.GetBlockType;
                        
                        if (hasDifferentTypes || anyIsBusy)
                            return false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        public override void UnsubscribeEvents()
        {
            OnMatchFound = delegate { };
        }
    }
}