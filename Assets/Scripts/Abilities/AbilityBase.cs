using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AbilityCooldown))]
public abstract class AbilityBase : MonoBehaviour
{
    [Header("Ability")]
    public UnityEvent OnCompleted;
    protected AbilityCooldown _owner;

    public void Init(AbilityCooldown abilityCooldown)
    {
        _owner = abilityCooldown;
    }

    public abstract void Activate();
}
