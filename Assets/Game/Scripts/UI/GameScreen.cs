using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Scripts.Data;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game.Scripts.Core
{
    public class GameScreen : ScreenViewBase
    {
        [SerializeField] private GridLayoutGroup blocksGrid;
        [SerializeField] private RectTransform blocksGridRect;
        [SerializeField] private BlockView blockViewPrefab;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private Transform swapContainer;
        [SerializeField] private ClickableView restartBtn;
        [SerializeField] private ClickableView nextLevelBtn;
        [SerializeField] private BalloonContainer balloonContainer;

        public Transform SwapContainer => swapContainer;
        public IClickableView RestartBtn => restartBtn;
        public IClickableView NextLevelBtn => nextLevelBtn;
        public BalloonContainer BalloonContainer => balloonContainer;


        public LevelMapData CreateMap(BlockType[,] mapData)
        {
            var mapXSize = mapData.GetLength(0);
            var mapYSize = mapData.GetLength(1);

            LevelMapData levelMapData = new(new(), new Vector2Int(mapXSize, mapYSize));
            
            var cellSize = (blocksGridRect.anchorMax.x - blocksGridRect.anchorMin.x) * Screen.width / mapXSize;
            blocksGrid.cellSize = new Vector2(cellSize, cellSize);

            for (int j = 0; j < mapYSize; j++)
            {
                for (int i = 0; i < mapXSize; i++)
                {
                    var newBlockView = Instantiate(blockViewPrefab, blocksGrid.transform);
                    newBlockView.SetType(mapData[i, j]);
                    levelMapData.LevelElements.Add(new Vector2Int(i, j), newBlockView);
                }
            }

            return levelMapData;
        }

        public void SetupLevelText(int levelArrayIndex)
        {
            var trueLevelNumber = levelArrayIndex + 1;
            levelText.text = $"Level {trueLevelNumber.ToString()}";
        }
    }
}