using UnityEngine;

namespace Game.Scripts.Data
{
    public struct DirectionData
    {
        public readonly Direction Direction;

        public DirectionData(Direction dir)
        {
            Direction = dir;
        }

        public Vector2Int GetOffset()
        {
            switch (Direction)
            {
                case Direction.Left:
                    return new Vector2Int(-1, 0);
                case Direction.Right:
                    return new Vector2Int(1, 0);
                case Direction.Bottom:
                    return new Vector2Int(0, -1);
                default: 
                    return new Vector2Int(0, 1);
            }
        }
    }
    
    public enum Direction
    {
        Left,
        Right,
        Top,
        Bottom,
    }
}