using UnityEngine;

public class AnimationSmoothFloat : StateMachineBehaviour
{
    [SerializeField] private string _parameterName;
    [SerializeField] private string _smoothParameterName;
    [SerializeField] private float _smoothTime = 0.2f;
    [SerializeField] private float _maxValue = 1f;

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float curValue = animator.GetFloat(_smoothParameterName);
        float targetValue = animator.GetFloat(_parameterName);

        curValue = Mathf.MoveTowards(curValue, targetValue, _maxValue / _smoothTime * Time.deltaTime);
        animator.SetFloat(_smoothParameterName, curValue);
    }
}
