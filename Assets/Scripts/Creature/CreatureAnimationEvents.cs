using UnityEngine;

public class CreatureAnimationEvents : MonoBehaviour
{
    [SerializeField] protected CreatureController _controller;
    [SerializeField] protected CreatureWeapon _weapon;

    public void Footstep()
    {
        _controller.PlayFootsteps();
    }

    public void Attack()
    {
        _weapon.Weapon.AttackAnimEvent();
    }
}
