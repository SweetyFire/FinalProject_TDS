using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public sealed class AbilityCooldown : MonoBehaviour
{
    [SerializeField] private float _cooldown;

    [Header("Input")]
    [SerializeField] private bool _enableMoveInput;
    [SerializeField] private bool _enableLookInput;
    [SerializeField] private bool _enableAttackInput;

    [Header("Casting")]
    [SerializeField] private float _castTime;
    [SerializeField] private bool _notLookTargetWhileCast;
    [SerializeField] private bool _notInterruptible;
    public float CastTime => _castTime;
    public float CastTimeRemaining => Mathf.Clamp(_castTimeRemaining, 0f, _castTime);
    public float CurrentCastTime => CastTime - CastTimeRemaining;
    public bool NotLookTargetWhileCast => _notLookTargetWhileCast;
    public bool NotInterruptible => _notInterruptible;
    public bool CanCasting => CastTime > 0f;

    [Header("Events")]
    public UnityEvent<AbilityCooldown> OnActivate;
    public UnityEvent<AbilityCooldown> OnCasted;
    public UnityEvent<AbilityCooldown> OnAllCompleted;
    public UnityEvent<AbilityCooldown> OnCastInterrupted;
    public UnityEvent<AbilityCooldown> OnCooldown;

    public float Cooldown => _cooldown;
    public float CooldownRemainingTime => Mathf.Clamp(_cooldownRemaining, 0f, _cooldown);
    public bool EnableMoveInput => _enableMoveInput;
    public bool EnableLookInput => _enableLookInput;
    public bool EnableAttackInput => _enableAttackInput;
    public AbilityCaster Caster => _caster;
    public bool CanActivate => _cooldownRemaining <= 0f;

    private float _cooldownRemaining = -1f;
    private float _castTimeRemaining = -1f;
    private AbilityCaster _caster;

    private List<AbilityBase> _abilities = new();
    private int _abilityCompletedCount;

    private void Update()
    {
        CooldownUpdate();
        CastingUpdate();
    }

    public void Init(AbilityCaster caster)
    {
        _caster = caster;
        InitAbilities();
    }

    public void Activate()
    {
        if (_cooldownRemaining > 0f) return;
        OnActivate?.Invoke(this);

        if (CanCasting)
        {
            _castTimeRemaining = _castTime;
        }
        else
        {
            _cooldownRemaining = _cooldown;
            OnCasted?.Invoke(this);
        }
    }

    public void InterruptCasting()
    {
        if (!CanCasting) return;
        _castTimeRemaining = -1f;
        OnCastInterrupted?.Invoke(this);
    }

    private void InitAbilities()
    {
        foreach (AbilityBase ability in GetComponents<AbilityBase>())
        {
            _abilities.Add(ability);
            ability.Init(this);
            ability.OnCompleted.AddListener(OnAbilityCompleted);
        }
    }

    private void CooldownUpdate()
    {
        if (_cooldownRemaining <= -1f) return;
        if (_cooldownRemaining > 0f)
        {
            _cooldownRemaining -= Time.deltaTime;
            return;
        }

        _cooldownRemaining = -1f;
        OnCooldown?.Invoke(this);
    }

    private void CastingUpdate()
    {
        if (!CanCasting) return;
        if (_castTimeRemaining <= -1f) return;
        if (_castTimeRemaining > 0f)
        {
            _castTimeRemaining -= Time.deltaTime;
            return;
        }

        _castTimeRemaining = -1f;
        _cooldownRemaining = _cooldown;
        OnCasted?.Invoke(this);
    }

    private void OnAbilityCompleted()
    {
        _abilityCompletedCount++;
        if (_abilityCompletedCount >= _abilities.Count)
        {
            _abilityCompletedCount = 0;
            OnAllCompleted?.Invoke(this);
        }
    }
}
