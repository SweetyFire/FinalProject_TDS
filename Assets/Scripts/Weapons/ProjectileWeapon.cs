using UnityEngine;

public class ProjectileWeapon : AimWeapon
{
    [Header("Projectile")]
    [SerializeField] private float _projectileFlySpeed = 1f;
    [SerializeField] private Projectile _projectilePrefab;

    protected override void AttackStart()
    {
        Projectile projectile = Instantiate(_projectilePrefab, _attackPoint.position, _owner.transform.rotation);
        projectile.Init(_projectileFlySpeed, _attackDistance, this);
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
