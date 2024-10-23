using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.Core
{
    public class BalloonContainer : View
    {
        [SerializeField] private BalloonView balloonPrefab;
        [SerializeField] private int balloonsCount;
        [SerializeField] private Vector2 balloonFlyDurationRange;
        [SerializeField] private float balloonXSwingOffset;

        private List<BalloonView> _balloonsList = new();

        public void StartBalloonsRoutine(CancellationToken token)
        {
            BalloonsRoutine(token).Forget();
        }

        private async UniTask BalloonsRoutine(CancellationToken token)
        {
            DestroyInstancedBalloons();

            for (int i = 0; i < balloonsCount; i++)
            {
                var balloon = Instantiate(balloonPrefab, transform);
                _balloonsList.Add(balloon);

                BalloonMoveRoutine(balloon, token).Forget();
                await UniTask.Delay(TimeSpan.FromSeconds(balloonFlyDurationRange.x), cancellationToken: token);
            }
        }


        private async UniTask BalloonMoveRoutine(BalloonView balloonView, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                balloonView.SetupRandomView();
                
                Vector2 startPos = GetRandomStartPosition();
                Vector2 endPos = GetRandomEndPosition();

                balloonView.transform.position = startPos;

                var duration = Random.Range(balloonFlyDurationRange.x, balloonFlyDurationRange.y);
                var elapsedTime = 0f;

                while (elapsedTime < duration)
                {
                    if (token.IsCancellationRequested) return;

                    elapsedTime += Time.deltaTime;
                    var t = elapsedTime / duration;

                    var swingsCount = duration / balloonFlyDurationRange.x; // [1; maxDur/minDur]
                    var sinWave = Mathf.Sin(t * swingsCount * Mathf.PI);

                    var currentPathPoint = Vector3.Lerp(startPos, endPos, t);
                    var swingOffset = new Vector3(sinWave * balloonXSwingOffset, 0, 0);

                    balloonView.transform.position = currentPathPoint + swingOffset;
                                                     ;

                    await UniTask.Yield();
                }
            }
        }

        private void DestroyInstancedBalloons()
        {
            foreach (var instancedBalloon in _balloonsList)
            {
                Destroy(instancedBalloon.gameObject);
            }

            _balloonsList.Clear();
        }

        private Vector2 GetRandomStartPosition()
        {
            var randomSide = Random.Range(0, 2) == 0 ? -1 : 1; // -1 left | 1 right
            var randomStartHeight = Random.Range(0f, 0.5f); // 0-50% of container height

            var xPos = RectTransform.position.x + RectTransform.rect.width / 2f * randomSide;
            var yPos = RectTransform.position.y - RectTransform.rect.height * randomStartHeight;

            return new Vector2(xPos, yPos);
        }

        private Vector2 GetRandomEndPosition()
        {
            var randomSide = Random.Range(0, 2) == 0 ? -1 : 1; // -1 left | 1 right
            var randomTargetX = Random.Range(0, 0.4f); //0-40% of container width offset

            var xPos = RectTransform.position.x + RectTransform.rect.width * randomTargetX * randomSide;
            var yPos = RectTransform.position.y + RectTransform.rect.height / 2f; //top of container

            return new Vector2(xPos, yPos);
        }
    }
}