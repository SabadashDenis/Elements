using System.Collections.Generic;
using Game.Scripts.Data;
using UnityEngine;

namespace Game.Scripts.Core
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "Game/Configs/LevelConfig")]
    public class LevelsConfig : ScriptableObject
    {
        [SerializeField] private List<LevelData> levelDatas = new();

        public IReadOnlyList<LevelData> LevelDatas => levelDatas;
    }
}