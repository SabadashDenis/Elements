namespace Game.Scripts.Core
{
    public interface ISystemContainer
    {
        TSystem Get<TSystem>()
            where TSystem : SystemBase;
    }
}