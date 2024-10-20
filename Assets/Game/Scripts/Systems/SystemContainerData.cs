namespace Game.Scripts.Core
{
    public class SystemContainerData
    {
        public readonly UISystem UI;
        public readonly GameBehaviorSystem GameBehavior;

        public SystemContainerData(UISystem ui, GameBehaviorSystem gameBehavior)
        {
            UI = ui;
            GameBehavior = gameBehavior;
        }
    }
}