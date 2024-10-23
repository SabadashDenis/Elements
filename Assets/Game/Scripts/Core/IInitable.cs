namespace Game.Scripts.Core
{
    public interface IInitable
    {
        void Init();
    }

    public interface IInitable<TData>
    {
        void Init(TData data);
    }
}