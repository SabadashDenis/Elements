using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.Scripts.Core
{
    public class BackgroundBehavior : GameBehaviorBase
    {
        private GameScreen _gameScreen;

        private CancellationTokenSource _cancellationTokenSource = new();
        
        protected override void OnInit(GameBehaviorData data)
        {
            _gameScreen = Data.UI.GetScreen<GameScreen>();

            _gameScreen.BalloonContainer.StartBalloonsRoutine(_cancellationTokenSource.Token);
        }


        private void OnDestroy()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}