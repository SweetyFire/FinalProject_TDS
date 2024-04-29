using UnityEngine;

public class ProjectileWeapon : AimWeapon
{
    [Header("Projectile")]
    [SerializeField] private float _projectileFlySpeed = 1f;
    [SerializeField] private float _projectileMaxFlyTime = 5f;
    [SerializeField] private Projectile _projectilePrefab;

    protected override void AttackStart()
    {
        Projectile projectile = Instantiate(_projectilePrefab, _attackPoint.position, _attackPoint.rotation);
        projectile.Init(_projectileFlySpeed, _projectileMaxFlyTime, this);
    }

#if UNITY_EDITOR
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        Gizmos.color = Color.red;
        if (Physics.Raycast(_attackPoint.position, _attackPoint.forward, out RaycastHit hit, _maxInAirDistance, _attackMask))
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(_attackPoint.position, _attackPoint.forward * hit.distance);
        }
        else
        {
            Gizmos.DrawRay(_attackPoint.position, _attackPoint.forward * _maxInAirDistance);
        }
    }
#endif
}
