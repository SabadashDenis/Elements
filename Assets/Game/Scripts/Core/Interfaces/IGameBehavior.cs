namespace Game.Scripts.Core
{
    public interface IGameBehavior
    {
        void DoUpdate();
        void DoFixedUpdate();
        void DoLateUpdate();
    }
}