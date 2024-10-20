using Game.Scripts.Core;
using Game.Scripts.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class BlockView : DragView
{
    [SerializeField, FoldoutGroup("Block Config")] private BlockConfig config;

    [SerializeField] private RectTransform view;
    [SerializeField] private Animator animator;

    private BlockType _type;

    public BlockType GetBlockType => _type;
    public RectTransform View => view;
    
    public void SetType(BlockType type)
    {
        _type = type;
        
        var typeData = config.GetDataForType(type);
        
        animator.runtimeAnimatorController = typeData.AnimatorController;

        var isVisible = type is not BlockType.Empty;
        view.gameObject.SetActive(isVisible);
        IsInteractable = isVisible;
    }
}
