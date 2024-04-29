using UnityEngine;

public class RayWeapon : AimWeapon
{
    [Header("Ray")]
    [SerializeField] private float _attackDistance;

    protected override void AttackStart()
    {
        if (Physics.Raycast(_attackPoint.position, _attackPoint.forward, out RaycastHit hit, _attackDistance, _attackMask))
        {
            if (hit.collider.TryGetComponent(out CreatureHealth health))
            {
                if (health.Team != _owner.Team)
                {
                    health.TakeDamage(_damage);
                }
            }
        }
    }

#if UNITY_EDITOR
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        Gizmos.color = Color.red;
        if (Physics.Raycast(_attackPoint.position, _attackPoint.forward, out RaycastHit hit, _attackDistance, _attackMask))
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(_attackPoint.position, _attackPoint.forward * hit.distance);
        }
        else
        {
            Gizmos.DrawRay(_attackPoint.position, _attackPoint.forward * _attackDistance);
        }
    }
#endif
}
