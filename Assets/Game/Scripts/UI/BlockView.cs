using Game.Scripts.Core;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class BlockView : DragView
{
    [SerializeField, FoldoutGroup("Block Config")] private BlockConfig config;

    [SerializeField] private Image view;
    [SerializeField] private Animator animator;

    public void SetType(BlockType type)
    {
        var typeData = config.GetDataForType(type);
        
        animator.runtimeAnimatorController = typeData.AnimatorController;

        var isVisible = type is not BlockType.Empty;
        view.gameObject.SetActive(isVisible);
        IsInteractable = isVisible;
    }
}
