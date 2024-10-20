using System.Collections.Generic;
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

        public Transform SwapContainer => swapContainer;
        
        public Dictionary<Vector2Int, BlockView> CreateMap(BlockType[,] mapData)
        {
            Dictionary<Vector2Int, BlockView> mapDictionary = new();

            var mapXSize = mapData.GetLength(0);
            var mapYSize = mapData.GetLength(1);

            var cellSize = blocksGridRect.rect.size.x / mapXSize;
            blocksGrid.cellSize = new Vector2(cellSize, cellSize);

            for (int i = 0; i < mapXSize; i++)
            {
                for (int j = 0; j < mapYSize; j++)
                {
                    var newBlockView = Instantiate(blockViewPrefab, blocksGrid.transform);
                    newBlockView.SetType(mapData[i,j]);
                    mapDictionary.Add(new Vector2Int(i,j), newBlockView);
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