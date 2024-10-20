using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Core
{
    public class GameBehaviorSystem : SystemBase
    {
        [SerializeField] private List<GameBehaviorBase> behaviorList;

        protected override void OnInit()
        {
            foreach (var gameBehavior in behaviorList)
            {
                gameBehavior.Init(new GameBehaviorData(Data.UI));
            }
        }
    }
}