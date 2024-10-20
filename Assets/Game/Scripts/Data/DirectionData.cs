using UnityEngine;

namespace Game.Scripts.Data
{
    public struct DirectionData
    {
        private readonly Direction _direction;

        public DirectionData(Direction dir)
        {
            _direction = dir;
        }

        public Vector2Int GetOffset()
        {
            switch (_direction)
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