using UnityEngine;

public class AnimationRandom : StateMachineBehaviour
{
    [SerializeField] private string _parameterName;
    [SerializeField] private int _minValue;
    [SerializeField] private int _maxValue;

    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        animator.SetInteger(_parameterName, Random.Range(_minValue, _maxValue));
    }
}
