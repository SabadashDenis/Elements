using System.Threading;

namespace Game.Scripts.Core
{
    public class GameBehaviorBase : InitableBehaviorBase<GameBehaviorData>
    {
        protected CancellationTokenSource _tokenSource = new();
        public CancellationToken Token => _tokenSource.Token;
        public GameBehaviorData BehaviorData => Data;
        
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