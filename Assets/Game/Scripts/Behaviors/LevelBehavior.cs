using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Scripts.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Core
{
    public class LevelBehavior : GameBehaviorBase
    {
        [SerializeField] private LevelsConfig levelsConfig;
        [SerializeField] private SwapAnimData swapAnimConfig;

        private GameScreen _gameScreen;

        private Dictionary<Vector2Int, BlockView> _currentMap = new();

        protected override void OnInit(GameBehaviorData data)
        {
            _gameScreen = data.UI.GetScreen<GameScreen>();

            LoadLevel(0);
        }

        private void LoadLevel(int index)
        {
            UnloadCurrentLevel();

            var levelData = levelsConfig.LevelDatas[index % levelsConfig.LevelDatas.Count];

            BlockType[,] mapData = new BlockType[levelData.MapSize.x, levelData.MapSize.y];

            foreach (var blockData in levelData.BlockDatas)
            {
                mapData[blockData.Position.x, blockData.Position.y] = blockData.Type;
            }

            _gameScreen.SetupLevelText(index);
            _currentMap = _gameScreen.CreateMap(mapData);

            foreach (var mapElement in _currentMap)
            {
                mapElement.Value.OnSwipe += (dir) => OnBlockSwiped(dir, mapElement);
            }
        }

        private async void OnBlockSwiped(DirectionData directionData, KeyValuePair<Vector2Int, BlockView> swipedElement)
        {
            var swipedBlockPos = swipedElement.Key;
            var swipedBlock = swipedElement.Value;

            var blockToSwapPos = swipedBlockPos + directionData.GetOffset();

            if (_currentMap.TryGetValue(blockToSwapPos, out var blockToSwap))
            {
                if (!(directionData.Direction is Direction.Up && blockToSwap.GetBlockType is BlockType.Empty))
                {
                    await DoBlocksSwap(swipedBlock, blockToSwap);
                    NormalizeMap();
                }
            }
        }

        private async UniTask DoBlocksSwap(BlockView first, BlockView second, bool withScale = true, int blocksChainLength = 1)
        {
            bool swapCompleted = false;
            
            var firstType = first.GetBlockType;
            var secondType = second.GetBlockType;

            var firstViewPos = first.View.position;
            var secondViewPos = second.View.position;

            var standardScale = first.View.localScale;

            Sequence swapSequence = DOTween.Sequence();

            if (withScale)
            {
                swapSequence
                    .Join(first.View.DOScale(standardScale * swapAnimConfig.ScaleInMultiplier, swapAnimConfig.ScaleDuration))
                    .Join(second.View.DOScale(standardScale * swapAnimConfig.ScaleInMultiplier, swapAnimConfig.ScaleDuration)
                        .OnComplete(MoveToContainer));
            }

            swapSequence
                .Append(first.View.DOMove(secondViewPos, swapAnimConfig.MoveDuration * blocksChainLength).SetEase(Ease.InOutSine))
                .Join(second.View.DOMove(firstViewPos, swapAnimConfig.MoveDuration * blocksChainLength).SetEase(Ease.InOutSine)
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
        }

        private void NormalizeMap()
        {
            foreach (var mapElement in _currentMap)
            {
                if (mapElement.Value.GetBlockType is not BlockType.Empty)
                {
                    DoFall(mapElement.Key);
                }
            }
        }

        private bool CanFallOneCell(Vector2Int mapElementPos, out Vector2Int underBlockPos)
        {
            underBlockPos = mapElementPos + new DirectionData(Direction.Down).GetOffset();

            if (_currentMap.TryGetValue(underBlockPos, out var bottomElement))
            {
                if (bottomElement.GetBlockType is BlockType.Empty)
                    return true;
            }

            return false;
        }

        private bool HasBlockAbove(Vector2Int mapElementPos, out Vector2Int aboveBlockPos)
        {
            aboveBlockPos = mapElementPos + new DirectionData(Direction.Up).GetOffset();

            if (_currentMap.TryGetValue(aboveBlockPos, out var topElement))
            {
                if (topElement.GetBlockType is not BlockType.Empty)
                    return true;
            }

            return false;
        }

        private async void DoFall(Vector2Int mapElementPos)
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
                
                if(HasBlockAbove(mapElementPos, out var topElementPos))
                    DoFall(topElementPos);
            }
        }

        private void UnloadCurrentLevel()
        {
            foreach (var pair in _currentMap)
            {
                if (pair.Value != null)
                {
                    pair.Value.RemoveAllOnSwipeEvents();
                    Destroy(pair.Value);
                }
            }

            _currentMap.Clear();
        }
    }
}