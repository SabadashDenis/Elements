using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace Game.Scripts.Core
{
    public interface IView
    {
        Tween Show();
        Tween Hide();
        void ShowImmediately();
        void HideImmediately();

    }
}