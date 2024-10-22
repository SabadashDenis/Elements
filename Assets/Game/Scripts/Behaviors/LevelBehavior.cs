using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Scripts.Data;
using UnityEngine;

namespace Game.Scripts.Core
{
    public class LevelBehavior : GameBehaviorBase
    {
        [SerializeField] private LevelsConfig levelsConfig;
        [SerializeField] private SwapAnimData swapAnimConfig;
        [SerializeField] private int blocksToMatch;
        [SerializeField] private int gameEndDelay;

        private GameScreen _gameScreen;

        private Dictionary<Vector2Int, BlockView> _currentMap = new();

        private int currentLevelIndex = -1;

        protected override void OnInit(GameBehaviorData data)
        {
            _gameScreen = data.UI.GetScreen<GameScreen>();

            LoadNextLevel();
        }

        private void LoadNextLevel()
        {
            UnloadCurrentLevel();
            
            currentLevelIndex++;

            var levelData = levelsConfig.LevelDatas[currentLevelIndex % levelsConfig.LevelDatas.Count];

            BlockType[,] mapData = new BlockType[levelData.MapSize.x, levelData.MapSize.y];

            foreach (var blockData in levelData.BlockDatas)
            {
                mapData[blockData.Position.x, blockData.Position.y] = blockData.Type;
            }

            _gameScreen.SetupLevelText(currentLevelIndex);
            _currentMap = _gameScreen.CreateMap(mapData);

            foreach (var mapElement in _currentMap)
            {
                mapElement.Value.OnSwipe += (dir) => OnBlockSwiped(dir, mapElement);
                mapElement.Value.OnBlockDestroy += CheckEndGame;
            }
        }

        private async void OnBlockSwiped(Direction swipeDirection, KeyValuePair<Vector2Int, BlockView> swipedElement)
        {
            var swipedBlockPos = swipedElement.Key;
            var swipedBlock = swipedElement.Value;


            var blockToSwapPos = swipedBlockPos + DirectionUtility.GetOffset(swipeDirection);

            if (_currentMap.TryGetValue(blockToSwapPos, out var blockToSwap))
            {
                if (swipedBlock.IsBusy || blockToSwap.IsBusy)
                    return;

                if (!(swipeDirection is Direction.Up && blockToSwap.GetBlockType is BlockType.Empty))
                {
                    await DoBlocksSwap(swipedBlock, blockToSwap);
                    CheckMatches();
                }
            }
        }

        private async UniTask DoBlocksSwap(BlockView first, BlockView second, bool withScale = true,
            int blocksChainLength = 1)
        {
            bool swapCompleted = false;

            var firstType = first.GetBlockType;
            var secondType = second.GetBlockType;

            var firstViewPos = first.View.position;
            var secondViewPos = second.View.position;

            var standardScale = first.View.localScale;

            first.SetBusy(true);
            second.SetBusy(true);

            Sequence swapSequence = DOTween.Sequence();

            if (withScale)
            {
                swapSequence
                    .Join(first.View.DOScale(standardScale * swapAnimConfig.ScaleInMultiplier,
                        swapAnimConfig.ScaleDuration))
                    .Join(second.View
                        .DOScale(standardScale * swapAnimConfig.ScaleInMultiplier, swapAnimConfig.ScaleDuration)
                        .OnComplete(MoveToContainer));
            }

            swapSequence
                .Append(first.View.DOMove(secondViewPos, swapAnimConfig.MoveDuration * blocksChainLength)
                    .SetEase(Ease.InOutSine))
                .Join(second.View.DOMove(firstViewPos, swapAnimConfig.MoveDuration * blocksChainLength)
                    .SetEase(Ease.InOutSine)
                    .OnComplete(SwitchViews));

            if (withScale)
            {
                swapSequence
                    .Append(first.View.DOScale(standardScale, swapAnimConfig.ScaleDuration))
                    .Join(second.View.DOScale(standardScale, swapAnimConfig.ScaleDuration));
            }

            swapSequence.OnComplete(() => swapCompleted = true);

            void MoveToContainer()
            {
                first.View.SetParent(_gameScreen.SwapContainer, true);
                second.View.SetParent(_gameScreen.SwapContainer, true);
            }

            void SwitchViews()
            {
                first.View.transform.SetParent(first.transform, true);
                second.View.transform.SetParent(second.transform, true);

                first.SetType(secondType);
                second.SetType(firstType);

                first.View.anchorMin = Vector2.zero;
                second.View.anchorMin = Vector2.zero;
                first.View.anchorMax = Vector2.one;
                second.View.anchorMax = Vector2.one;
                first.View.anchoredPosition = Vector2.zero;
                second.View.anchoredPosition = Vector2.zero;
            }

            await UniTask.WaitUntil(() => swapCompleted);

            first.SetBusy(false);
            second.SetBusy(false);
        }

        private async UniTask NormalizeMap()
        {
            List<UniTask> normalizeTasks = new();

            foreach (var mapElement in _currentMap)
            {
                if (mapElement.Value.GetBlockType is not BlockType.Empty)
                {
                    normalizeTasks.Add(DoFall(mapElement.Key));
                }
            }

            await UniTask.WaitWhile(() => normalizeTasks.Any((task) => !task.GetAwaiter().IsCompleted));
        }

        private async void CheckMatches()
        {
            await NormalizeMap();
            
            List<Vector2Int> biggestMatchGroup = new();

            foreach (var mapElement in _currentMap)
            {
                if(!HasThreeInRow(mapElement.Key))
                    continue;
                
                List<Vector2Int> currentCheckedGroup = new();
                CollectNeighbors(mapElement.Key, ref currentCheckedGroup);

                if (currentCheckedGroup.Count > biggestMatchGroup.Count)
                {
                    biggestMatchGroup.Clear();
                    biggestMatchGroup.AddRange(currentCheckedGroup);
                }
            }

            if (biggestMatchGroup.Count < blocksToMatch)
                return;

            List<UniTask> destroyTasks = new();
            
            foreach (var groupElementPos in biggestMatchGroup)
            {
                if (_currentMap.TryGetValue(groupElementPos, out var groupElement))
                {
                    destroyTasks.Add(groupElement.Destroy());
                }
            }

            CheckMatches();
        }

        private bool HasThreeInRow(Vector2Int checkedPos)
        {
            _currentMap.TryGetValue(checkedPos, out var target);
            
            _currentMap.TryGetValue(checkedPos + DirectionUtility.GetOffset(Direction.Up), out var top);
            _currentMap.TryGetValue(checkedPos + DirectionUtility.GetOffset(Direction.Down), out var bottom);
            _currentMap.TryGetValue(checkedPos + DirectionUtility.GetOffset(Direction.Left), out var left);
            _currentMap.TryGetValue(checkedPos + DirectionUtility.GetOffset(Direction.Right), out var right);

            var topMatch = target?.GetBlockType == top?.GetBlockType;
            var bottomMatch = target?.GetBlockType == bottom?.GetBlockType;
            var leftMatch = target?.GetBlockType == left?.GetBlockType;
            var rightMatch = target?.GetBlockType == right?.GetBlockType;

            return ((topMatch && bottomMatch) || (leftMatch && rightMatch));
        }
        

        private void CollectNeighbors(Vector2Int checkedPos, ref List<Vector2Int> collectList)
        {
            if (_currentMap.TryGetValue(checkedPos, out var checkedElement))
            {
                if (checkedElement.GetBlockType is BlockType.Empty)
                    return;

                foreach (var direction in DirectionUtility.AllDirections)
                {
                    var neighborPos = checkedPos + DirectionUtility.GetOffset(direction);

                    if (_currentMap.TryGetValue(neighborPos, out var neighborElement))
                    {
                        var hasSameTypes = neighborElement.GetBlockType == checkedElement.GetBlockType;
                        var notCollected = !collectList.Contains(neighborPos);

                        if (hasSameTypes && notCollected)
                        {
                            collectList.Add(neighborPos);
                            CollectNeighbors(neighborPos, ref collectList);
                        }
                    }
                }
            }
        }

        private bool CanFallOneCell(Vector2Int mapElementPos, out Vector2Int underBlockPos)
        {
            underBlockPos = mapElementPos + DirectionUtility.GetOffset(Direction.Down);

            if (_currentMap.TryGetValue(underBlockPos, out var bottomElement))
            {
                if (bottomElement.GetBlockType is BlockType.Empty)
                    return true;
            }

            return false;
        }

        private bool HasBlockAbove(Vector2Int mapElementPos, out Vector2Int aboveBlockPos)
        {
            aboveBlockPos = mapElementPos + DirectionUtility.GetOffset(Direction.Up);

            if (_currentMap.TryGetValue(aboveBlockPos, out var topElement))
            {
                if (topElement.GetBlockType is not BlockType.Empty)
                    return true;
            }

            return false;
        }

        private async UniTask DoFall(Vector2Int mapElementPos)
        {
            Vector2Int resultPos = mapElementPos;

            int cellsToFall = 0;

            while (CanFallOneCell(resultPos, out var underBlockPos))
            {
                resultPos = underBlockPos;
                cellsToFall++;
            }

            if (resultPos != mapElementPos)
            {
                _currentMap.TryGetValue(mapElementPos, out var fallenBlock);
                _currentMap.TryGetValue(resultPos, out var lastBottomBlock);

                await DoBlocksSwap(fallenBlock, lastBottomBlock, false, cellsToFall);

                if (HasBlockAbove(mapElementPos, out var topElementPos))
                    await DoFall(topElementPos);
            }
        }

        private async void CheckEndGame()
        {
            var isMapEmpty = _currentMap.All(mapElement =>
                mapElement.Value.GetBlockType is BlockType.Empty && !mapElement.Value.IsBusy);

            if (isMapEmpty)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(gameEndDelay));
                LoadNextLevel();
            }
        }

        private void UnloadCurrentLevel()
        {
            foreach (var pair in _currentMap)
            {
                if (pair.Value != null)
                {
                    pair.Value.UnsubscribeEvents();
                    Destroy(pair.Value.gameObject);
                }
            }

            _currentMap.Clear();
        }
    }
}