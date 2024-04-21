using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeapon : CreatureWeapon
{
    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled)
        {
            AttackEnd();
        }
        else if (!ctx.started)
        {
            AttackStart();
        }
    }
}
