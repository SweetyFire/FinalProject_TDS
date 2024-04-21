using Unity.VisualScripting;
using UnityEngine;

public abstract class AimWeapon : Weapon
{
    [Header("Aim")]
    [SerializeField] protected Transform _attackPoint;

    protected float _maxInAirDistance = 100f;
    protected float _minInAirDistance = 3f;
    protected float _maxInAirRadius = 0.1f;

    private void FixedUpdate()
    {
        AimFixedUpdate();
    }

    private void AimFixedUpdate()
    {
        if (_owner == null) return;

        Vector3 lookPos;
        Vector3 castPos = _owner.transform.position + (Vector3.up * _owner.Controller.Height / 2f);
        if (Physics.SphereCast(castPos, _maxInAirRadius, _owner.transform.forward, out RaycastHit hit, _maxInAirDistance))
        {
            if (hit.distance > _minInAirDistance)
            {
                lookPos = (_owner.transform.forward * hit.distance) + castPos;
            }
            else
            {
                lookPos = (_owner.transform.forward * _minInAirDistance) + castPos;
            }
        }
        else
        {
            lookPos = (_owner.transform.forward * _maxInAirDistance) + castPos;
        }

        lookPos.y = _attackPoint.position.y;
        Vector3 direction = (lookPos - _attackPoint.position).normalized;
        _attackPoint.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }

#if UNITY_EDITOR
    protected virtual void OnDrawGizmosSelected()
    {
        if (_owner == null) return;

        Gizmos.color = Color.red;
        Vector3 castPos = _owner.transform.position + (Vector3.up * _owner.Controller.Height / 2f);
        if (Physics.SphereCast(castPos, _maxInAirRadius, _owner.transform.forward, out RaycastHit hit, _maxInAirDistance))
        {
            float dist;
            if (hit.distance > _minInAirDistance)
            {
                dist = hit.distance;
            }
            else
            {
                dist = _minInAirDistance;
            }

            Gizmos.DrawRay(castPos, _owner.transform.forward * dist);
            Gizmos.DrawWireSphere(_owner.transform.forward * dist + castPos, _maxInAirRadius);
        }
        else
        {
            Gizmos.DrawRay(castPos, _owner.transform.forward * _maxInAirDistance);
            Gizmos.DrawWireSphere(_owner.transform.forward * _maxInAirDistance + castPos, _maxInAirRadius);
        }
    }
#endif
}
