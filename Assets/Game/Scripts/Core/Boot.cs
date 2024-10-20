using UnityEngine;
using UnityEngine.Serialization;


namespace Game.Scripts.Core
{
    public class Boot : MonoBehaviour
    {
        [SerializeField] private SystemContainer systemContainer;
        [SerializeField] private UISystem uiSystem;
        [SerializeField] private GameBehaviorSystem gameBehaviorSystem;

        private void Awake()
        {
            systemContainer.Init(new SystemContainerData(uiSystem, gameBehaviorSystem));
        }
    }
}