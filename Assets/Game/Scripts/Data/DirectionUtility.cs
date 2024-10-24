using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Data
{
    public static class DirectionUtility
    {
        public static readonly List<Direction> AllDirections = new()
            { Direction.Up, Direction.Down, Direction.Left, Direction.Right };

        public static readonly List<Direction> VerticalDirections = new()
            { Direction.Up, Direction.Down};
        
        public static readonly List<Direction> HorizontalDirections = new()
            { Direction.Left, Direction.Right };
        
        public static Vector2Int GetOffset(Direction dir)
        {
            switch (dir)
            {
                case Direction.Left:
                    return new Vector2Int(-1, 0);
                case Direction.Right:
                    return new Vector2Int(1, 0);
                case Direction.Down:
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
        Up,
        Down,
    }
}