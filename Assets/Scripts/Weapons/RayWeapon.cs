using UnityEngine;

public class RayWeapon : AimWeapon
{
    [Header("Ray")]
    [SerializeField] private float _attackDistance;
    [SerializeField] private float _attackRadius;

    protected override void AttackStart()
    {
        if (Physics.SphereCast(_attackPoint.position, _attackRadius, _attackPoint.forward, out RaycastHit hit, _attackDistance))
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
        if (Physics.SphereCast(_attackPoint.position, _attackRadius, _attackPoint.forward, out RaycastHit hit, _attackDistance))
        {
            Gizmos.DrawRay(_attackPoint.position, _attackPoint.forward * hit.distance);
            Gizmos.DrawWireSphere(_attackPoint.forward * hit.distance + _attackPoint.position, _attackRadius);
        }
        else
        {
            Gizmos.DrawRay(_attackPoint.position, _attackPoint.forward * _attackDistance);
            Gizmos.DrawWireSphere(_attackPoint.forward * _attackDistance + _attackPoint.position, _attackRadius);
        }
    }
#endif
}
