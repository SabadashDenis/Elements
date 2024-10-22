using UnityEngine;

public class DestroyAnimBehavior : StateMachineBehaviour
{
    private float _destroyAnimDuration = 0;

    public float DestroyAnimDuration => _destroyAnimDuration;
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _destroyAnimDuration = stateInfo.length;
    }
}
