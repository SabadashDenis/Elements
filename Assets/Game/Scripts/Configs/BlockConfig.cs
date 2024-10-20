using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Data;
using UnityEditor.Animations;
using UnityEngine;

namespace Game.Scripts.Core
{
    [CreateAssetMenu(fileName = "BlockConfig", menuName = "Game/Configs/BlockConfig")]
    public class BlockConfig : ScriptableObject
    {
        [SerializeField] private List<BlockConfigData> blockConfigDatas = new();

        public BlockConfigData GetDataForType(BlockType type) => 
            blockConfigDatas.FirstOrDefault((target) => target.Type == type);
    }

    [Serializable]
    public struct BlockConfigData
    {
        [SerializeField] private BlockType type;
        [SerializeField] private AnimatorController animatorController;

        public BlockType Type => type;
        public AnimatorController AnimatorController => animatorController;
    }
}