using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

public class AnimatorExtension : MonoBehaviour
{
    public void SetBoolTrueAnimator(string paramName) => GetComponent<Animator>().SetBool(paramName, true);
    public void SetBoolFalseAnimator(string paramName) => GetComponent<Animator>().SetBool(paramName, false);

    public void _ChangeCurrentAnimator(RuntimeAnimatorController newAnimator) => GetComponent<Animator>().runtimeAnimatorController = newAnimator;

#if UNITY_EDITOR
    [Button("SetAllTransitionTo0Blend")]
    void RemoveAllTransitionData()
    {
        var animator = GetComponent<Animator>();
        if (animator == null) return;

        AnimatorController masterAnimator = animator.runtimeAnimatorController as AnimatorController;
        AnimatorStateMachine asm = masterAnimator.layers[0].stateMachine;

        foreach (var el in asm.states.SelectMany(i => i.state.transitions).Concat(asm.anyStateTransitions))
        {
            el.hasExitTime = false;
            el.exitTime = 0;
            el.hasFixedDuration = false;
            el.duration = 0;
            UnityEditor.EditorUtility.SetDirty(el);
        }
        UnityEditor.EditorUtility.SetDirty(asm);
        UnityEditor.EditorUtility.SetDirty(masterAnimator);
    }
#endif

}
