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


        public Dictionary<Vector2Int, BlockView> CreateMap(BlockType[,] mapData)
        {
            Dictionary<Vector2Int, BlockView> mapDictionary = new();

            var mapXSize = mapData.GetLength(0);
            var mapYSize = mapData.GetLength(1);

            var cellSize = (blocksGridRect.anchorMax.x - blocksGridRect.anchorMin.x) * Screen.width / mapXSize;
            blocksGrid.cellSize = new Vector2(cellSize, cellSize);

            for (int i = 0; i < mapXSize; i++)
            {
                for (int j = 0; j < mapYSize; j++)
                {
                    var newBlockView = Instantiate(blockViewPrefab, blocksGrid.transform);
                    newBlockView.SetType(mapData[j, i]);
                    mapDictionary.Add(new Vector2Int(i, j), newBlockView);
                }
            }

            return mapDictionary;
        }

        public void SetupLevelText(int levelArrayIndex)
        {
            var trueLevelNumber = levelArrayIndex + 1;
            levelText.text = $"Level {trueLevelNumber.ToString()}";
        }
    }
}