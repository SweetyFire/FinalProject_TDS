using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AbilityCaster : MonoBehaviour
{
    [Header("Caster")]
    [SerializeField] private Transform _abilitiesParent;
    [SerializeField] private List<AbilityCooldown> _abilitiyPrefabs;

    [Header("UI")]
    [SerializeField] private BarUI _abilityCastBar;

    [Header("Events")]
    public UnityEvent OnCastStart;
    public UnityEvent OnCastEnd;

    public CreatureController Controller => _controller;
    public IReadOnlyList<AbilityCooldown> Abilities => _abilities;
    public bool IsCasting => _isCasting;
    public bool NotLookTargetWhileCast => _castingAbility != null && _castingAbility.NotLookTargetWhileCast;
    public AbilityCooldown CastingAbility => _castingAbility;

    private List<AbilityCooldown> _abilities = new();
    private CreatureController _controller;
    private CreatureWeapon _weapon;

    private bool _isCasting;
    private AbilityCooldown _castingAbility;

    private void Update()
    {
        AbilityCastBarUpdate();
    }

    public void Init(CreatureController controller)
    {
        _controller = controller;
        DisableAbilityBar();
        InitAbilities();
    }

    public void Init(CreatureController controller, CreatureWeapon weapon)
    {
        _weapon = weapon;
        Init(controller);
    }

    public void ActivateAbility(int index)
    {
        InterruptAbilityCasting();

        if (CanCasting(index))
        {
            if (CanActivate(index))
                OnCastStart?.Invoke();

            if (!CanCastAndAttack(index))
            {
                if (_weapon.IsAttacking) return;
            }
        }

        _abilities[index].Activate();
    }

    public bool CanActivate(int index)
    {
        return _abilities[index].CanActivate;
    }

    public bool CanCasting(int index)
    {
        return _abilities[index].CanCasting;
    }

    public bool CanCastAndAttack(int index)
    {
        if (!_abilities[index].CanCasting) return true;
        if (_abilities[index].EnableAttackInput) return true;

        return false;
    }

    public bool CanCastAndAttack()
    {
        if (!IsCasting) return true;
        if (_castingAbility == null) return true;
        if (!_castingAbility.CanCasting) return true;
        if (_castingAbility.EnableAttackInput) return true;

        return false;
    }

    public void InterruptAbilityCasting()
    {
        if (!IsCasting) return;
        if (_castingAbility == null) return;
        if (!_castingAbility.CanCasting) return;

        _castingAbility.InterruptCasting();
        OnAbilityCompleted(_castingAbility);
        OnAbilityCastEnd(_castingAbility);
    }

    private void AbilityCastBarUpdate()
    {
        if (!IsCasting) return;
        UpdateAbilityCastBarValue();
    }

    private void InitAbilities()
    {
        foreach (AbilityCooldown ability in _abilitiyPrefabs)
        {
            AbilityCooldown abil = Instantiate(ability, _abilitiesParent);
            _abilities.Add(abil);
            abil.Init(this);

            abil.OnActivate.AddListener(OnAbilityCastStart);
            abil.OnCasted.AddListener(OnAbilityCastEnd);
            abil.OnAllCompleted.AddListener(OnAbilityCompleted);
            abil.OnCastInterrupted.AddListener(OnAbilityCastEnd);
        }
    }

    protected void OnAbilityCastStart(AbilityCooldown ability)
    {
        if (_isCasting) return;

        if (ability.CanCasting)
        {
            _castingAbility = ability;
            _isCasting = true;
            _abilityCastBar.gameObject.SetActive(true);
        }
        else
        {
            if (!ability.EnableLookInput)
            {
                _controller.DisableLook();
            }

            if (!ability.EnableMoveInput)
            {
                _controller.DisableMove();
            }

            if (!ability.EnableAttackInput)
            {
                _weapon.DisableAttack();
            }
        }
    }

    protected void OnAbilityCastEnd(AbilityCooldown ability)
    {
        if (ability != null)
        {
            if (ability.CanCasting)
            {
                OnCastEnd?.Invoke();

                if (!ability.EnableLookInput)
                {
                    _controller.DisableLook();
                }

                if (!ability.EnableMoveInput)
                {
                    _controller.DisableMove();
                }

                if (!ability.EnableAttackInput)
                {
                    _weapon.DisableAttack();
                }
            }
        }

        _isCasting = false;
        _castingAbility = null;

        if (_abilityCastBar.gameObject.activeSelf)
            _abilityCastBar.gameObject.SetActive(false);
    }

    protected void OnAbilityCompleted(AbilityCooldown ability)
    {
        _controller.EnableMove();
        _controller.EnableLook();
        _weapon.EnableAttack();
    }

    private void UpdateAbilityCastBarValue()
    {
        _abilityCastBar.UpdateValue(CastingAbility.CurrentCastTime, CastingAbility.CastTime);
    }

    private void DisableAbilityBar()
    {
        if (!_abilityCastBar.gameObject.activeSelf) return;
        _abilityCastBar.gameObject.SetActive(false);
    }
}
