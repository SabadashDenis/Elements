using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Scripts.Core
{
    public class GameBehaviorSystem : SystemBase
    {
        [SerializeField] private List<GameBehaviorBase> behaviorList = new();

        public SaveData GetSaveData
        {
            get
            {
                var levelBehavior = GetBehavior<LevelBehavior>();

                if (levelBehavior != null)
                {
                    return new SaveData
                    {
                        MapState = levelBehavior.GetMapState,
                        LevelIndex = levelBehavior.GetCurrentLevelIndex
                    };
                }

                return null;
            }
        }

        protected override void OnInit()
        {
            foreach (var gameBehavior in behaviorList)
            {
                gameBehavior.Init(new GameBehaviorData(Data.GetSystem<UISystem>(), Data.GetSystem<SaveSystem>()));
            }
        }

        private TBehavior GetBehavior<TBehavior>() where TBehavior : GameBehaviorBase
        {
            foreach (var behavior in behaviorList)
            {
                if (behavior is TBehavior tBehavior)
                {
                    return tBehavior;
                }
            }

            return null;
        }
    }
}