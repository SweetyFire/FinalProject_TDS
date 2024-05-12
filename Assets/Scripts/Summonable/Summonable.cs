using UnityEngine;

public abstract class Summonable : MonoBehaviour
{
    protected AbilityCaster _owner;

    public virtual void Init(AbilityCaster caster)
    {
        _owner = caster;
    }
}
