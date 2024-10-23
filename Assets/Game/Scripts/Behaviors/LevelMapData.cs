using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Scripts.Core
{
    public struct LevelMapData
    {
        public readonly List<LevelElementData> LevelElements;

        public LevelMapData(List<LevelElementData> levelElements)
        {
            LevelElements = levelElements;
        }

        public bool TryGetElementData(Vector2Int elementPos, out LevelElementData elementData)
        {
            if (LevelElements.Any((item) => item.ElementPos == elementPos))
            {
                elementData = LevelElements.First((item) => item.ElementPos == elementPos);
                return true;
            }

            elementData = new LevelElementData();
            return false;
        }
        
        public bool TryGetElementData(BlockView elementView, out LevelElementData elementData)
        {
            if (LevelElements.Any((item) => item.ElementView.Equals(elementView)))
            {
                elementData = LevelElements.First((item) => item.ElementView.Equals(elementView));
                return true;
            }

            elementData = new LevelElementData();
            return false;
        }
    }

    public struct LevelElementData
    {
        public readonly Vector2Int ElementPos;
        public readonly BlockView ElementView;

        public LevelElementData(Vector2Int elementPos, BlockView elementView)
        {
            ElementPos = elementPos;
            ElementView = elementView;
        }
    }
}