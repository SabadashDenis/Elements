using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Core;
using Game.Scripts.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class BlockView : DragView
{
    [SerializeField, FoldoutGroup("Block Config")]
    private BlockConfig config;

    [SerializeField] private RectTransform view;
    [SerializeField] private Animator animator;

    private bool _isBusy;
    private BlockType _type;
    private DestroyAnimBehavior _destroyAnimBehavior;
    private DestroyAnimBehavior DestroyAnimBehavior =>
        _destroyAnimBehavior ??= animator.GetBehaviour<DestroyAnimBehavior>();
    public BlockType GetBlockType => _isBusy ? BlockType.Empty : _type;
    public RectTransform View => view;
    public bool IsBusy => _isBusy;
    
    public event Action OnBlockDestroy = delegate { };

    public void SetBusy(bool condition)
    {
        _isBusy = condition;
    }

    public void SetType(BlockType type)
    {
        _type = type;

        var typeData = config.GetDataForType(type);

        animator.runtimeAnimatorController = typeData.AnimatorController;

        var isVisible = type is not BlockType.Empty;
        view.gameObject.SetActive(isVisible);
        IsInteractable = isVisible;
    }

    public async UniTask Destroy(CancellationToken token)
    {
        SetBusy(true);
        
        animator.Play("Destroy");
        animator.Update(0);
        
        await UniTask.Delay(TimeSpan.FromSeconds(DestroyAnimBehavior.DestroyAnimDuration), cancellationToken: token);
        SetType(BlockType.Empty);
        
        SetBusy(false);
        
        OnBlockDestroy.Invoke();
    }

    public void UnsubscribeEvents()
    {
        OnBlockDestroy = delegate { };
        RemoveAllOnSwipeEvents();
    }
}