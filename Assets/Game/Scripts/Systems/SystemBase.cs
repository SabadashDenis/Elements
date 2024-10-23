using System;
using UnityEngine;

namespace Game.Scripts.Core
{
    public abstract class SystemBase : SystemBase<SystemData>
    {
        
    }

    public abstract class SystemBase<SystemData> : MonoBehaviour, IInitable<SystemData>
    {
        private bool _isInitialized;
            
        protected SystemData Data;

        public void Init(SystemData data)
        {
            if (!_isInitialized)
            {
                Data = data;
                _isInitialized = true;
                OnInit();
            }
        }

        protected abstract void OnInit();
    }

    public class SystemData
    {
        private readonly SystemContainer _container;
        
        public TSystem GetSystem<TSystem>()
            where TSystem : SystemBase => _container.Get<TSystem>();
        
        public SystemData(SystemContainer container)
        {
            _container = container;
        }
    }
}