using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeapon : CreatureWeapon
{
    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            AttackStart();
        }
        else if (ctx.canceled)
        {
            AttackEnd();
        }
    }
}
