using UnityEngine;

public class RayWeapon : Weapon
{
    [Header("Ray")]
    [SerializeField] private float _attackDistance;
    [SerializeField] private float _attackRadius;
    [SerializeField] protected Transform _attackPoint;

    public override void Attack()
    {
        if (_timeToAttack > 0f) return;

        _timeToAttack = _attackRate;

        Debug.Log("Pow!");
        if (Physics.SphereCast(_attackPoint.position, _attackRadius, _attackPoint.forward, out RaycastHit hit, _attackDistance))
        {
            if (hit.collider.TryGetComponent(out CreatureHealth health))
            {
                if (health.Team != _owner.Team)
                {
                    health.TakeDamage(_damage);
                    Debug.Log($"{health.gameObject.name} attacked!");
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
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
}
