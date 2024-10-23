using System;
using System.Collections.Generic;
using Game.Scripts.Data;

namespace Game.Scripts.Core
{
    [Serializable]
    public class SaveData
    {
        public LevelData MapState;
        public int LevelIndex;
    }
}