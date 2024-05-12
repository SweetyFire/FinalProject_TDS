using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeapon : CreatureWeapon
{
    private AbilityCaster _abilityCaster;

    protected override void InitComponents()
    {
        base.InitComponents();
        _abilityCaster = GetComponent<AbilityCaster>();
    }

    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled)
        {
            AttackEnd();
        }
        else if (!ctx.started)
        {
            if (!_abilityCaster.CanCastAndAttack())
                _abilityCaster.InterruptAbilityCasting();

            AttackStart();
        }
    }
}
