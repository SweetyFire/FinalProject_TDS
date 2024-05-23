using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySight : MonoBehaviour
{
    [SerializeField] private EnemyController _controller;

    [Header("Rotation")]
    [SerializeField] private float _minRotationSpeed = 100f;
    [SerializeField] private float _maxRotationSpeed = 500f;

    [Header("Sight")]
    [SerializeField] private float _viewingAngle = 90f;
    [SerializeField] private LayerMask _sightLayerMask;

    public bool SeeCurrentTarget => _seeCurrentTarget;
    public CreatureHealth Target => _target;

    private bool _seeCurrentTarget;
    private CreatureHealth _target;
    private NavMeshAgent _agent;
    private SphereCollider _collider;

    private bool _isRotatingToPosition;
    private Vector3 _rotatePosition;

    private List<CreatureHealth> _creatureInSight = new();

    private void Awake()
    {
        InitComponents();
    }

    private void Update()
    {
        SightUpdate();
        LookUpdate();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Target != null) return;

        if (!other.TryGetComponent(out CreatureHealth creature)) return;
        _creatureInSight.Add(creature);

        if (creature is not PlayerHealth player) return;
        if (!IsGoalWithinReach(player)) return;

        NotifyAllies(player);
        ChangeTarget(player);
    }

    private void OnTriggerExit(Collider other)
    {
        if (Target != null) return;

        if (!other.TryGetComponent(out CreatureHealth creature)) return;

        int findIndex = _creatureInSight.IndexOf(creature);
        if (findIndex < 0) return;

        _creatureInSight.RemoveAt(findIndex);
    }

    public void RotateTo(Vector3 position)
    {
        if (Target != null) return;

        _isRotatingToPosition = true;
        _rotatePosition = position;
    }

    public bool IsGoalWithinReach(CreatureHealth enemy)
    {
        Vector3 direction = GetDirectionTo(enemy.transform.position);
        if (GetAngle(direction) > _viewingAngle)
            return false;

        if (!SeesTarget(enemy))
            return false;

        return true;
    }

    public bool IsGoalWithinReach(Vector3 position, float maxDistance, out Vector3 navMeshPosition)
    {
        navMeshPosition = position;
        NavMesh.SamplePosition(position, out NavMeshHit hit, maxDistance, NavMesh.AllAreas);

        NavMeshPath path = new();
        if (!_agent.CalculatePath(hit.position, path))
            return false;

        navMeshPosition = hit.position;
        return true;
    }

    public Vector3 GetLookDirection()
    {
        if (Target == null)
        {
            if (_isRotatingToPosition)
            {
                Vector3 direction = GetDirectionTo(_rotatePosition);
                Debug.DrawRay(transform.position, direction, Color.red, 5f);
                if (GetAngle(direction) <= 20f)
                {
                    _isRotatingToPosition = false;
                }

                return direction;
            }
        }

        Vector3 lookDirection = GetDirectionTo(_agent.steeringTarget);

        if (Target == null)
            return lookDirection;
        else if (_agent.remainingDistance > _agent.stoppingDistance * 2f)
            return lookDirection;
        else
            return (Target.transform.position - _controller.transform.position).normalized;
    }

    private void InitComponents()
    {
        _collider = GetComponent<SphereCollider>();
        _agent = _controller.GetComponent<NavMeshAgent>();
    }

    private void ChangeTarget(CreatureHealth target)
    {
        if (Target != null) return;

        _target = target;
        _controller.SetChaseState();
        target.BattleStart();
    }

    private void NotifyAllies(CreatureHealth target)
    {
        if (Target != null) return;

        foreach (CreatureHealth creature in _creatureInSight)
        {
            if (creature is not EnemyHealth enemy) continue;
            enemy.Controller.Sight.ChangeTarget(target);
        }
    }

    private void SightUpdate()
    {
        if (_controller.DisabledLookInput) return;
        if (!_controller.CanTakeAction()) return;
        if (_creatureInSight.Count <= 0) return;

        if (Target != null)
        {
            _isRotatingToPosition = false;
            _seeCurrentTarget = SeesTarget(Target);
            _controller.ChangeStoppingDistance();
        }
        else
        {
            _seeCurrentTarget = false;
            for (int i = _creatureInSight.Count - 1; i >= 0; i--)
            {
                if (_creatureInSight[i] == null)
                {
                    _creatureInSight.RemoveAt(i);
                    continue;
                }

                if (_creatureInSight[i] is EnemyHealth) continue;
                if (!IsGoalWithinReach(_creatureInSight[i])) continue;

                NotifyAllies(_creatureInSight[i]);
                ChangeTarget(_creatureInSight[i]);
                break;
            }
        }
    }

    private void LookUpdate()
    {
        if (_controller.DisabledLookInput) return;

        Vector3 direction = GetLookDirection();
        float rotationSpeed;
        if (!_controller.IsStopped())
        {
            float rotationSpeedDelta = Mathf.Clamp01(GetAngle(direction) / _viewingAngle);
            rotationSpeed = Mathf.Lerp(_minRotationSpeed, _maxRotationSpeed, rotationSpeedDelta);
        }
        else
        {
            rotationSpeed = _maxRotationSpeed;
        }

        if (direction == Vector3.zero) return;
        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
        _controller.transform.rotation = Quaternion.RotateTowards(_controller.transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }

    private Vector3 GetDirectionTo(Vector3 position)
    {
        return (position - _controller.transform.position).normalized;
    }

    public float GetAngle(Vector3 direction)
    {
        return Vector3.Angle(transform.forward, direction);
    }

    private bool SeesTarget(CreatureHealth enemy)
    {
        return !Physics.Linecast(transform.position, enemy.Controller.CenterPosition, _sightLayerMask);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (_collider == null)
            _collider = GetComponent<SphereCollider>();

        Gizmos.color = Color.red;
        Vector3 rayDirection = Quaternion.Euler(0f, _viewingAngle / 2f, 0f) * transform.forward;
        Gizmos.DrawRay(transform.position, rayDirection * _collider.radius);

        rayDirection = Quaternion.Euler(0f, -(_viewingAngle / 2f), 0f) * transform.forward;
        Gizmos.DrawRay(transform.position, rayDirection * _collider.radius);
    }
#endif
}
