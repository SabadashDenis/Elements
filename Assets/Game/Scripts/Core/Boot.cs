using UnityEngine;

namespace Game.Scripts.Core
{
    public class Boot : MonoBehaviour
    {
        [SerializeField] private SystemContainer systemContainer;
        [SerializeField] private UISystem uiSystem;
        [SerializeField] private GameBehaviorSystem gameBehaviorSystem;
        [SerializeField] private SaveSystem saveSystem;

        private void Awake()
        {
            systemContainer.Init(new SystemContainerData(uiSystem, gameBehaviorSystem, saveSystem));
        }

        private void OnDestroy()
        {
            saveSystem.Save();
        }
    }
}