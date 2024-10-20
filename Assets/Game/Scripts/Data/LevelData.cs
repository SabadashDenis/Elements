using System;
using System.Collections.Generic;
using Game.Scripts.Core;
using UnityEngine;

namespace Game.Scripts.Data
{
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
}