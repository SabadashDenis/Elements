using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Scripts.Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Scripts.Core
{
    public class LevelBehavior : GameBehaviorBase
    {
        [SerializeField] private SwapHandler swapHandler;
        [SerializeField] private LevelHandler levelHandler;

        private GameScreen _gameScreen;

        private int _currentLevelIndex = 0;

        private CancellationTokenSource _cancellationTokenSource = new();

        public LevelData GetMapState => levelHandler.GetMapState;

        public int CurrentLevelIndex => _currentLevelIndex;

        protected override void OnInit(GameBehaviorData data)
        {
            _gameScreen = data.UI.GetScreen<GameScreen>();
            
            /*_gameScreen.RestartBtn.OnClickEvent += () => LoadLevel(_currentLevelIndex);
            _gameScreen.NextLevelBtn.OnClickEvent += () =>
            {
                _currentLevelIndex++;
                LoadLevel(_currentLevelIndex);
            };*/
            
            //LoadLevel(_currentLevelIndex, SaveSystem.GetCurrentSave);

            ConnectLoadLevelHandler();
            
            levelHandler.LoadLevelInitial();
            
            _gameScreen.RestartBtn.OnClickEvent += levelHandler.RestartLevel;
            _gameScreen.NextLevelBtn.OnClickEvent += levelHandler.LoadNextLevel;
        }

        private void ConnectLoadLevelHandler()
        {
            levelHandler.Init(new(_gameScreen, _cancellationTokenSource));

            levelHandler.OnLevelUnload += StopAllRoutines;
            levelHandler.OnLevelLoaded += ConnectSwapHandler;
        }

        private void ConnectSwapHandler(LevelMapData levelMapData)
        {
            swapHandler.Init(new(levelMapData, _cancellationTokenSource));
        }

        private void StopAllRoutines()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new();
        }


        /*private void LoadLevel(int index, SaveData saveData = null)
        {
            UnloadCurrentLevel();

            LevelData levelData = new LevelData();

            if (saveData != null)
            {
                levelData = saveData.MapState;
                index = saveData.LevelIndex;
                _currentLevelIndex = index;
            }
            else
            {
                levelData = levelsConfig.LevelDatas[index % levelsConfig.LevelDatas.Count];
            }

            BlockType[,] mapData = new BlockType[levelData.MapSize.x, levelData.MapSize.y];

            foreach (var blockData in levelData.BlockDatas)
            {
                mapData[blockData.Position.x, blockData.Position.y] = blockData.Type;
            }

            _gameScreen.SetupLevelText(index);
            _currentMap = _gameScreen.CreateMap(mapData);

            foreach (var mapElement in _currentMap.LevelElements)
            {
                //mapElement.Value.OnSwipe += (dir) => OnBlockSwiped(dir, mapElement);
                mapElement.ElementView.OnBlockDestroy += CheckEndGame;
            }
        }*/

        /*private async void OnBlockSwiped(Direction swipeDirection, KeyValuePair<Vector2Int, BlockView> swipedElement)
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
        }*/

        /*private async UniTask DoBlocksSwap(BlockView first, BlockView second, bool withScale = true,
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

            await UniTask.WaitUntil(() => swapCompleted, cancellationToken: _cancellationTokenSource.Token);

            if(_cancellationTokenSource.IsCancellationRequested)
                return;
            
            first.SetBusy(false);
            second.SetBusy(false);
        }*/

        private async UniTask<bool> NormalizeMap()
        {
            bool isNormalized = false;

            List<UniTask> normalizeTasks = new();

            foreach (var mapElement in levelHandler.CurrentMap.LevelElements)
            {
                if (mapElement.ElementView.GetBlockType is not BlockType.Empty)
                {
                    //normalizeTasks.Add(DoFall(mapElement.Key));
                }
            }

            if (normalizeTasks.Count == 0)
                isNormalized = true;

            await UniTask.WaitWhile(() => normalizeTasks.Any((task) => !task.GetAwaiter().IsCompleted),
                cancellationToken: _cancellationTokenSource.Token);

            return isNormalized;
        }

        private async UniTaskVoid CheckMatches()
        {
            var isMapNormalized = await NormalizeMap();

            List<Vector2Int> biggestMatchGroup = new();

            foreach (var mapElement in levelHandler.CurrentMap.LevelElements)
            {
                if (!HasThreeInRow(mapElement.ElementPos))
                    continue;

                List<Vector2Int> currentCheckedGroup = new();
                CollectNeighbors(mapElement.ElementPos, ref currentCheckedGroup);

                if (currentCheckedGroup.Count > biggestMatchGroup.Count)
                {
                    biggestMatchGroup.Clear();
                    biggestMatchGroup.AddRange(currentCheckedGroup);
                }
            }

            List<UniTask> destroyTasks = new();

            foreach (var groupElementPos in biggestMatchGroup)
            {
                if (levelHandler.CurrentMap.TryGetElementData(groupElementPos, out var groupElement))
                {
                    destroyTasks.Add(groupElement.ElementView.Destroy(_cancellationTokenSource.Token));
                }
            }

            if (isMapNormalized && destroyTasks.Count == 0)
                return;

            await UniTask.WaitWhile(() => destroyTasks.Any(task => !task.GetAwaiter().IsCompleted),
                cancellationToken: _cancellationTokenSource.Token);

            CheckMatches().Forget();
        }

        private bool HasThreeInRow(Vector2Int checkedPos)
        {
            levelHandler.CurrentMap.TryGetElementData(checkedPos, out var target);

            levelHandler.CurrentMap.TryGetElementData(checkedPos + DirectionUtility.GetOffset(Direction.Up), out var top);
            levelHandler.CurrentMap.TryGetElementData(checkedPos + DirectionUtility.GetOffset(Direction.Down), out var bottom);
            levelHandler.CurrentMap.TryGetElementData(checkedPos + DirectionUtility.GetOffset(Direction.Left), out var left);
            levelHandler.CurrentMap.TryGetElementData(checkedPos + DirectionUtility.GetOffset(Direction.Right), out var right);

            var topMatch = target.ElementView?.GetBlockType == top.ElementView?.GetBlockType;
            var bottomMatch = target.ElementView?.GetBlockType == bottom.ElementView?.GetBlockType;
            var leftMatch = target.ElementView?.GetBlockType == left.ElementView?.GetBlockType;
            var rightMatch = target.ElementView?.GetBlockType == right.ElementView?.GetBlockType;

            return ((topMatch && bottomMatch) || (leftMatch && rightMatch));
        }


        private void CollectNeighbors(Vector2Int checkedPos, ref List<Vector2Int> collectList)
        {
            if (levelHandler.CurrentMap.TryGetElementData(checkedPos, out var checkedElement))
            {
                if (checkedElement.ElementView.GetBlockType is BlockType.Empty)
                    return;

                foreach (var direction in DirectionUtility.AllDirections)
                {
                    var neighborPos = checkedPos + DirectionUtility.GetOffset(direction);

                    if (levelHandler.CurrentMap.TryGetElementData(neighborPos, out var neighborElement))
                    {
                        var hasSameTypes = neighborElement.ElementView.GetBlockType ==
                                           checkedElement.ElementView.GetBlockType;
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

            if (levelHandler.CurrentMap.TryGetElementData(underBlockPos, out var bottomElement))
            {
                if (bottomElement.ElementView.GetBlockType is BlockType.Empty && !bottomElement.ElementView.IsBusy)
                    return true;
            }

            return false;
        }

        private bool HasBlockAbove(Vector2Int mapElementPos, out Vector2Int aboveBlockPos)
        {
            aboveBlockPos = mapElementPos + DirectionUtility.GetOffset(Direction.Up);

            if (levelHandler.CurrentMap.TryGetElementData(aboveBlockPos, out var topElement))
            {
                if (topElement.ElementView.GetBlockType is not BlockType.Empty)
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
                levelHandler.CurrentMap.TryGetElementData(mapElementPos, out var fallenBlock);
                levelHandler.CurrentMap.TryGetElementData(resultPos, out var lastBottomBlock);

                //await DoBlocksSwap(fallenBlock, lastBottomBlock, false, cellsToFall);

                if (HasBlockAbove(mapElementPos, out var topElementPos))
                    await DoFall(topElementPos);
            }
        }

        /*private void CheckEndGame()
        {
            var isMapEmpty = loadLevelHandler.CurrentMap.LevelElements.All(mapElement =>
                mapElement.ElementView.GetBlockType is BlockType.Empty && !mapElement.ElementView.IsBusy);

            if (isMapEmpty)
            {
                _currentLevelIndex++;
                LoadLevel(_currentLevelIndex);
            }
        }*/
    }
}