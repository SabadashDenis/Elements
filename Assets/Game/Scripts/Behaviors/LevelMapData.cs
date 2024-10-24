using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Scripts.Core
{
    public class LevelMapData
    {
        public readonly Dictionary<Vector2Int, BlockView> LevelElements = new ();
        public readonly Vector2Int MapSize = new ();

        public LevelMapData() { }
        
        public LevelMapData(Dictionary<Vector2Int, BlockView> levelElements, Vector2Int mapSize)
        {
            LevelElements = levelElements;
            MapSize = mapSize;
        }

        /*public bool TryGetElementData(Vector2Int elementPos, out LevelElementData elementData)
        {
            if (LevelElements.Any((item) => item.ElementPos == elementPos))
            {
                elementData = LevelElements.First((item) => item.ElementPos == elementPos);
                return true;
            }

            elementData = new LevelElementData();
            return false;
        }*/
        
        /*public bool TryGetElementData(BlockView elementView, out LevelElementData elementData)
        {
            if (LevelElements.Any((item) => item.ElementView.Equals(elementView)))
            {
                elementData = LevelElements.First((item) => item.ElementView.Equals(elementView));
                return true;
            }

            elementData = new LevelElementData();
            return false;
        }*/
    }

    /*public struct LevelElementData
    {
        public readonly Vector2Int ElementPos;
        public readonly BlockView ElementView;

        public LevelElementData(Vector2Int elementPos, BlockView elementView)
        {
            ElementPos = elementPos;
            ElementView = elementView;
        }
    }*/
}