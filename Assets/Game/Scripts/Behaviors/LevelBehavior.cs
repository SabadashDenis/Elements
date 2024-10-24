using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Game.Scripts.Data;
using UnityEngine;

namespace Game.Scripts.Core
{
    public class LevelBehavior : GameBehaviorBase
    {
        [SerializeField] private SwapHandler swapHandler;
        [SerializeField] private LevelHandler levelHandler;
        [SerializeField] private FallHandler fallHandler;
        [SerializeField] private CheckMatchHandler matchHandler;

        private GameScreen _gameScreen;

        private CancellationTokenSource _cancellationTokenSource = new();

        public LevelData GetMapState => levelHandler.GetMapState;

        public int GetCurrentLevelIndex => levelHandler.CurrentLevelIndex;

        protected override void OnInit(GameBehaviorData data)
        {
            _gameScreen = data.UI.GetScreen<GameScreen>();

            levelHandler.Init(new(_gameScreen, _cancellationTokenSource));
            
            levelHandler.OnLevelLoaded += ConnectSwapHandler;
            levelHandler.OnLevelLoaded += ConnectFallHandler;
            levelHandler.OnLevelLoaded += ConnectMatchHandler;
            levelHandler.OnLevelUnload += OnLevelUnload;

            _gameScreen.RestartBtn.OnClickEvent += levelHandler.RestartLevel;
            _gameScreen.NextLevelBtn.OnClickEvent += levelHandler.LoadNextLevel;
            
            levelHandler.LoadLevelInitial();
        }

        private void ConnectSwapHandler(LevelMapData levelMapData)
        {
            swapHandler.Init(new(levelMapData, _cancellationTokenSource));
            swapHandler.OnSwapReleased += fallHandler.StartFallRoutine;
        }

        private void ConnectFallHandler(LevelMapData levelMapData)
        {
            fallHandler.Init( new(levelMapData, _cancellationTokenSource));
            fallHandler.OnFallCompleted += matchHandler.StartCheckMatchRoutine;
        }

        private void ConnectMatchHandler(LevelMapData levelMapData)
        {
            matchHandler.Init(new(levelMapData, _cancellationTokenSource));
            matchHandler.OnMatchFound += CheckEndGame;
            matchHandler.OnMatchFound += fallHandler.StartFallRoutine;
        }

        private void CheckEndGame()
        {
            var isMapEmpty = levelHandler.CurrentMap.LevelElements.All(mapElement =>
                mapElement.Value.GetBlockType is BlockType.Empty && !mapElement.Value.IsBusy);

            if (isMapEmpty)
            {
                levelHandler.LoadNextLevel();
            }
        }

        private void OnLevelUnload()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new();
            
            swapHandler.UnsubscribeEvents();
            fallHandler.UnsubscribeEvents();
            matchHandler.UnsubscribeEvents();
        }
    }
}