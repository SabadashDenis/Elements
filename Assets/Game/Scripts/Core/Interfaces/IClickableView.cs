using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Scripts.Core
{
    public interface IClickableView
    {
        event Action OnDownEvent;
        event Action OnUpEvent;
        event Action OnClickEvent;

        void RemoveAllOnDownEvents();
        void RemoveAllOnUpEvents();
        void RemoveAllOnClickEvents();
        
        UniTask WaitForClick(CancellationToken token);
    }
}