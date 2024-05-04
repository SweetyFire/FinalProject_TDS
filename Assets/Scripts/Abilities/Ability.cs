using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    [SerializeField] protected float _cooldown;

    public float Cooldown => _cooldown;
    public float RemainingCooldownTime => _cooldownTime;

    protected AbilityCaster _owner;
    private float _cooldownTime;

    private void Update()
    {
        CooldownUpdate();
    }

    public void Init(AbilityCaster caster)
    {
        _owner = caster;
    }

    public void Activate()
    {
        if (_cooldownTime > 0f) return;

        _cooldownTime = _cooldown;
        ActivateAbility();
    }

    protected abstract void ActivateAbility();

    private void CooldownUpdate()
    {
        if (_cooldownTime > 0f)
        {
            _cooldownTime -= Time.deltaTime;
        }
    }
}
