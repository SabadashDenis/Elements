using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Scripts.Core
{
    public class GameBehaviorSystem : SystemBase
    {
        [SerializeField] private List<GameBehaviorBase> behaviorList;

        public SaveData GetSaveData
        {
            get
            {
                var resultSaveData = new SaveData();

                var levelBehavior = behaviorList.FirstOrDefault((behavior => behavior is LevelBehavior)) as LevelBehavior;

                resultSaveData.MapState = levelBehavior.GetMapState;
                resultSaveData.LevelIndex = levelBehavior.GetCurrentLevelIndex;

                return resultSaveData;
            }
        }

        protected override void OnInit()
        {
            foreach (var gameBehavior in behaviorList)
            {
                gameBehavior.Init(new GameBehaviorData(Data.GetSystem<UISystem>(), Data.GetSystem<SaveSystem>()));
            }
        }
    }
}