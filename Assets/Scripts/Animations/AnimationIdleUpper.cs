using UnityEngine;

public class AnimationIdleUpper : StateMachineBehaviour
{
    [SerializeField] private float _smoothTime = 0.2f;
    private LTDescr _anim;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float curValue = animator.GetLayerWeight(layerIndex);
        if (curValue == 0f) return;

        if (_anim != null)
            LeanTween.cancel(_anim.id);

       
        _anim = LeanTween.value(curValue, 0f, _smoothTime).setOnUpdate(val => WeightValueUpdate(animator, layerIndex, val));
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float curValue = animator.GetLayerWeight(layerIndex);
        if (curValue == 1f) return;

        if (_anim != null)
            LeanTween.cancel(_anim.id);

        _anim = LeanTween.value(curValue, 1f, _smoothTime).setOnUpdate(val => WeightValueUpdate(animator, layerIndex, val));
    }

    private void WeightValueUpdate(Animator animator, int layerIndex, float value)
    {
        animator.SetLayerWeight(layerIndex, value);
    }
}
