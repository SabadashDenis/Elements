using System;
using Game.Scripts.Core;
using UnityEngine;

namespace Game.Scripts.Data
{
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
}