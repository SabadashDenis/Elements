using System;
using System.Collections.Generic;
using System.Threading;
using Game.Scripts.Data;
using UnityEngine;

namespace Game.Scripts.Core
{
    public class LevelHandler : HandlerBase<GameScreen, CancellationTokenSource>
    {
        [SerializeField] private LevelsConfig levelsConfig;

        public event Action<LevelMapData> OnLevelLoaded = delegate { };
        public event Action OnLevelUnload = delegate { };

        private int _currentLevelIndex = 0;
        private bool _saveLoaded;

        private LevelMapData _currentMapCache = new();

        public LevelMapData CurrentMap => _currentMapCache;

        public LevelData GetMapState
        {
            get
            {
                var mapSize = levelsConfig.LevelDatas[_currentLevelIndex % levelsConfig.LevelDatas.Count].MapSize;
                List<BlockData> blockDatas = new();

                foreach (var mapElement in _currentMapCache.LevelElements)
                {
                    var blockData = new BlockData(mapElement.Value.GetBlockType,
                        new Vector2Int(mapElement.Key.x, mapElement.Key.y));
                    blockDatas.Add(blockData);
                }

                return new LevelData(mapSize, blockDatas);
            }
        }

        protected override void OnBeforeHandle()
        {
        }

        public void LoadLevelInitial() => LoadLevel(_currentLevelIndex, true);

        public void LoadNextLevel() => LoadLevel(++_currentLevelIndex);

        public void RestartLevel() => LoadLevel(_currentLevelIndex);

        private void LoadLevel(int levelIndex, bool checkSave = false)
        {
            UnloadCurrentLevel();

            var levelData = GetLevelData(levelIndex);

            if (checkSave && TryGetLevelSave(out var saveData))
                levelData = saveData;

            BlockType[,] mapData = new BlockType[levelData.MapSize.x, levelData.MapSize.y];

            foreach (var blockData in levelData.BlockDatas)
            {
                mapData[blockData.Position.x, blockData.Position.y] = blockData.Type;
            }

            Data.HandlerData.SetupLevelText(levelIndex);
            _currentMapCache = Data.HandlerData.CreateMap(mapData);
            
            OnLevelLoaded.Invoke(_currentMapCache);
        }

        private bool TryGetLevelSave(out LevelData fromSaveData)
        {
            var saveData = SaveSystem.GetCurrentSave;

            if (saveData != null && !_saveLoaded)
            {
                _saveLoaded = true;
                _currentLevelIndex = saveData.LevelIndex;
                fromSaveData = saveData.MapState;
                return true;
            }

            fromSaveData = new LevelData();
            return false;
        }

        private LevelData GetLevelData(int levelIndex)
        {
            return levelsConfig.LevelDatas[levelIndex % levelsConfig.LevelDatas.Count];
        }

        private void UnloadCurrentLevel()
        {
            OnLevelUnload.Invoke();

            foreach (var pair in _currentMapCache.LevelElements)
            {
                if (pair.Value != null)
                {
                    pair.Value.UnsubscribeEvents();
                    Destroy(pair.Value.gameObject);
                }
            }

            _currentMapCache.LevelElements.Clear();
        }
    }
}