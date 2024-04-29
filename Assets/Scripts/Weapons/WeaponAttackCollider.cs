using UnityEngine;

public class WeaponAttackCollider : MonoBehaviour
{
    private MeleeWeapon _owner;

    private void Awake()
    {
        _owner = GetComponentInParent<MeleeWeapon>();
    }

    private void OnTriggerEnter(Collider other)
    {
        _owner.OnCreatureEnter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        _owner.OnCreatureExit(other);
    }
}
