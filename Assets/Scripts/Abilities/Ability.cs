using System;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    [Header("Ability")]
    [SerializeField] protected float _cooldown;
    [SerializeField] protected bool _enableMoveInput;
    [SerializeField] protected bool _enableLookInput;

    public float Cooldown => _cooldown;
    public float RemainingCooldownTime => _cooldownTime;
    public bool EnableMoveInput => _enableMoveInput;
    public bool EnableLookInput => _enableLookInput;

    public event Action<Ability> OnStarted;
    public abstract event Action<Ability> OnCompleted;

    protected AbilityCaster _owner;
    private float _cooldownTime;

    protected virtual void Update()
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
        OnStarted?.Invoke(this);
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
