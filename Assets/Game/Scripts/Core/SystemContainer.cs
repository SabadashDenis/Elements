using System;
using System.Collections.Generic;

namespace Game.Scripts.Core
{
    public class SystemContainer : InitableBehaviorBase<SystemContainerData>, ISystemContainer
    { 
        private readonly List<SystemBase> _systems = new();
        
        protected override void OnInit(SystemContainerData data)
        {
            var systems = GetComponentsInChildren<SystemBase>(true);

            var systemData = new SystemData(this);
            
            foreach (var system in systems)
            {
                system.Init(systemData);
                _systems.Add(system);
            }
        }

        public TSystem Get<TSystem>() where TSystem : SystemBase
        {
            foreach (var system in _systems)
            {
                if (system is TSystem tSystem)
                    return tSystem;
            }

            return null;
        }
    }
}