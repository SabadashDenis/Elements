using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Scripts.Core
{
    public abstract class HandlerBase<T, T1> : InitableBehaviorBase<HandlerDataContainer<T, T1>> where T1 : CancellationTokenSource
    {
        protected override void OnInit(HandlerDataContainer<T, T1> dataContainer)
        {
            OnBeforeHandle();
            //HandleRoutine();
        }

        protected abstract void OnBeforeHandle();

        //protected abstract UniTask HandleRoutine();
    }

    public class HandlerDataContainer<T,T1>
    {
        public T HandlerData { get; private set; }
        public T1 TokenSource { get; private set; }

        public HandlerDataContainer(T handlerData, T1 tokenSource)
        {
            HandlerData = handlerData;
            TokenSource = tokenSource;
        }
    }
}