using UnityEngine;

public class BulletWeapon : AimWeapon
{
    [Header("Bullet")]
    [SerializeField] private float _bulletFlySpeed = 1f;
    [SerializeField] private float _bulletMaxFlyTime = 5f;
    [SerializeField] private Bullet _bulletPrefab;

    protected override void AttackStart()
    {
        Bullet bullet = Instantiate(_bulletPrefab, _attackPoint.position, _attackPoint.rotation);
        bullet.Init(_bulletFlySpeed, _bulletMaxFlyTime, this);
    }

#if UNITY_EDITOR
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        Gizmos.color = Color.red;
        if (Physics.SphereCast(_attackPoint.position, _maxInAirRadius, _attackPoint.forward, out RaycastHit hit, _maxInAirDistance))
        {
            Gizmos.DrawRay(_attackPoint.position, _attackPoint.forward * hit.distance);
            Gizmos.DrawWireSphere(_attackPoint.forward * hit.distance + _attackPoint.position, _maxInAirRadius);
        }
        else
        {
            Gizmos.DrawRay(_attackPoint.position, _attackPoint.forward * _maxInAirDistance);
            Gizmos.DrawWireSphere(_attackPoint.forward * _maxInAirDistance + _attackPoint.position, _maxInAirRadius);
        }
    }
#endif
}
