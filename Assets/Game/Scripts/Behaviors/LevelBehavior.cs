using System;
using System.Collections.Generic;
using Game.Scripts.Data;
using UnityEngine;

namespace Game.Scripts.Core
{
    public class LevelBehavior : GameBehaviorBase
    {
        [SerializeField] private LevelsConfig levelsConfig;

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
                var firstBlockType = swipedBlock.GetBlockType;
                var secondBlockType = blockToSwap.GetBlockType;
                
                swipedBlock.SetType(secondBlockType);
                blockToSwap.SetType(firstBlockType);
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

    [Serializable]
    public struct LevelData
    {
        public Vector2Int MapSize;
        public List<BlockData> BlockDatas;

        public LevelData(Vector2Int mapSize, List<BlockData> blockDatas)
        {
            MapSize = mapSize;
            BlockDatas = blockDatas;
        }
    }

    [Serializable]
    public struct BlockData
    {
        public BlockType Type;
        public Vector2Int Position;

        public BlockData(BlockType type, Vector2Int position)
        {
            Type = type;
            Position = position;
        }
    }

    [Serializable]
    public enum BlockType
    {
        Empty = 0,
        Fire = 1,
        Water = 2
    }
}