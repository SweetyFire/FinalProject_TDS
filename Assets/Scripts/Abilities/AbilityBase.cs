using UnityEngine;
using UnityEngine.Events;

public abstract class AbilityBase : MonoBehaviour
{
    [Header("Ability")]
    public UnityEvent OnCompleted;
    protected AbilityCaster _owner;

    public void Init(AbilityCaster caster)
    {
        _owner = caster;
    }

    public abstract void Activate();
}
