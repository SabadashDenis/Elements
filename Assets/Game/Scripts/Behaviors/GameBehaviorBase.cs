namespace Game.Scripts.Core
{
    public class GameBehaviorBase : InitableBehaviorBase<GameBehaviorData>, IGameBehavior
    {

        public void DoUpdate()
        {
            if (IsInitialized)
            {
                OnUpdate();
            }
        }

        public void DoFixedUpdate()
        {
            if (IsInitialized)
            {
                OnFixedUpdate();
            }
        }

        public void DoLateUpdate()
        {
            if (IsInitialized)
            {
                OnLateUpdate();
            }
        }
        
        
        public virtual void OnUpdate() { }

        public virtual void OnFixedUpdate() { }

        public virtual void OnLateUpdate() { }
        protected override void OnInit(GameBehaviorData data) { }
    }

    public struct GameBehaviorData
    {
        public readonly UISystem UI;
        public readonly SaveSystem Save;

        public GameBehaviorData(UISystem ui, SaveSystem save)
        {
            UI = ui;
            Save = save;
        }
    }
}