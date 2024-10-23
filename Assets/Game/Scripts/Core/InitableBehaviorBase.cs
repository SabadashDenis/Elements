using System.Threading;
using UnityEngine;

namespace Game.Scripts.Core
{
    public abstract class InitableBehaviorBase : MonoBehaviour, IInitable
    {
        private bool _isInitialized;

        protected bool IsInitialized => _isInitialized;

        public virtual void Init()
        {
            if (!_isInitialized)
            {
                _isInitialized = true;
                OnInit();
            }
        }

        protected abstract void OnInit();
    }

    public abstract class InitableBehaviorBase<TData> : MonoBehaviour, IInitable<TData>
    {
        private bool _isInitialized;
        protected bool IsInitialized => _isInitialized;
        protected TData Data;
        
        public void Init(TData data)
        {
            if (!_isInitialized)
            {
                Data = data;
                _isInitialized = true;
                OnInit(data);
            }
        }
        
        protected abstract void OnInit(TData data);
    }
}