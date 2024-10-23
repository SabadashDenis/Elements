namespace Game.Scripts.Core
{
    public class SystemContainerData
    {
        public readonly UISystem UI;
        public readonly GameBehaviorSystem Game;
        public readonly SaveSystem Save;

        public SystemContainerData(UISystem ui, GameBehaviorSystem game, SaveSystem save)
        {
            UI = ui;
            Game = game;
            Save = save;
        }
    }
}