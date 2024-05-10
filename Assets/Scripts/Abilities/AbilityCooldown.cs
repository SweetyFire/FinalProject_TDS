using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public sealed class AbilityCooldown : MonoBehaviour
{
    [Header("Cooldown")]
    [SerializeField] private float _cooldown;
    [SerializeField] private bool _enableMoveInput;
    [SerializeField] private bool _enableLookInput;

    public UnityEvent<AbilityCooldown> OnActivate;
    public UnityEvent<AbilityCooldown> OnCooldown;
    public UnityEvent<AbilityCooldown> OnAllCompleted;

    public float Cooldown => _cooldown;
    public float CooldownRemainingTime => Mathf.Clamp(_cooldownRemaining, 0f, _cooldown);
    public bool EnableMoveInput => _enableMoveInput;
    public bool EnableLookInput => _enableLookInput;
    public AbilityCaster Caster => _caster;

    private float _cooldownRemaining;
    private AbilityCaster _caster;

    private List<AbilityBase> _abilities = new();
    private int _abilityCompletedCount;

    private void Update()
    {
        CooldownUpdate();
    }

    public void Init(AbilityCaster caster)
    {
        _caster = caster;
        InitAbilities();
    }

    public void Activate()
    {
        if (_cooldownRemaining > 0f) return;
        _cooldownRemaining = _cooldown;
        OnActivate?.Invoke(this);
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
