using System;
using System.Collections.Generic;
using DG.Tweening;
using Game.Scripts.Data;
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

        private void OnBlockSwiped(DirectionData directionData, KeyValuePair<Vector2Int, BlockView> swipedElement)
        {
            var swipedBlockPos = swipedElement.Key;
            var swipedBlock = swipedElement.Value;

            var blockToSwapPos = swipedBlockPos + directionData.GetOffset();

            if (_currentMap.TryGetValue(blockToSwapPos, out var blockToSwap))
            {
                if (!(directionData.Direction is Direction.Top && blockToSwap.GetBlockType is BlockType.Empty))
                    DoBlocksSwap(swipedBlock, blockToSwap);
            }
        }

        private void DoBlocksSwap(BlockView first, BlockView second)
        {
            var firstType = first.GetBlockType;
            var secondType = second.GetBlockType;

            var firstViewPos = first.View.position;
            var secondViewPos = second.View.position;

            var standardScale = first.View.localScale;


            Sequence swapSequence = DOTween.Sequence()
                .Join(first.View.DOScale(standardScale * swapAnimConfig.ScaleInMultiplier, swapAnimConfig.ScaleDuration))
                .Join(second.View.DOScale(standardScale * swapAnimConfig.ScaleInMultiplier, swapAnimConfig.ScaleDuration)
                    .OnComplete(MoveToContainer))
                .Append(first.View.DOMove(secondViewPos, swapAnimConfig.MoveDuration).SetEase(Ease.InOutSine))
                .Join(second.View.DOMove(firstViewPos, swapAnimConfig.MoveDuration).SetEase(Ease.InOutSine)
                    .OnComplete(SwitchViews))
                .Append(first.View.DOScale(standardScale, swapAnimConfig.ScaleDuration))
                .Join(second.View.DOScale(standardScale, swapAnimConfig.ScaleDuration));

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