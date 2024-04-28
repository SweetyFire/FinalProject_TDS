using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : CreatureController
{
    [SerializeField] private float _minRotationSpeed = 5f;
    [SerializeField] private float _maxRotationSpeed = 16f;
    [SerializeField] private float _viewingAngle = 90f;
    [SerializeField] private LayerMask _obstaclesMask;

    private NavMeshAgent _agent;
    private Rigidbody _rb;

    private float _actionTimer;
    private float _timeBetweenActions = 0.5f;
    private CreatureController _currentTarget;
    private List<CreatureController> _enemiesInSight = new();

    private void Awake()
    {
        InitComponents();
    }

    private void Update()
    {
        SightUpdate();
        WalkUpdate();
        LookUpdate();
        ActionTimerUpdate();
    }

    public void OnEnemyEnterTrigger(Collider other)
    {
        if (!other.TryGetComponent(out PlayerController player)) return;
        _enemiesInSight.Add(player);
        if (!IsGoalWithinReach(player)) return;

        _currentTarget = player;
    }

    public void OnEnemyExitTrigger(Collider other)
    {
        if (!other.TryGetComponent(out PlayerController player)) return;
        if (!_enemiesInSight.Contains(player)) return;
        _enemiesInSight.Remove(player);
    }

    protected override void InitComponents()
    {
        base.InitComponents();
        _agent = GetComponent<NavMeshAgent>();
        _rb = GetComponent<Rigidbody>();
    }

    private void SightUpdate()
    {
        if (!CanTakeAction()) return;
        if (_currentTarget != null) return;
        if (_enemiesInSight.Count <= 0) return;

        foreach (CreatureController enemy in _enemiesInSight)
        {
            if (!IsGoalWithinReach(enemy)) continue;
            _currentTarget = enemy;
            break;
        }
    }

    private bool IsGoalWithinReach(CreatureController enemy)
    {
        Vector3 direction = enemy.transform.position - transform.position;
        if (GetAngle(direction) > _viewingAngle)
            return false;

        if (Physics.Linecast(transform.position + Vector3.up, enemy.transform.position + Vector3.up, _obstaclesMask))
            return false;

        return true;
    }

    private void WalkUpdate()
    {
        if (!CanTakeAction()) return;
        if (_currentTarget == null) return;

        _agent.destination = _currentTarget.transform.position;
    }

    private bool CanTakeAction()
    {
        if (_actionTimer >= _timeBetweenActions)
            return true;

        return false;
    }

    private void ActionTimerUpdate()
    {
        if (_actionTimer > 0)
        {
            _actionTimer -= Time.deltaTime;
            return;
        }

        _actionTimer = _timeBetweenActions;
    }

    private void LookUpdate()
    {
        if (_currentTarget == null) return;

        Vector3 direction = GetLookDirection();
        float rotationSpeed;
        if (!IsStopped())
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
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }

    private Vector3 GetLookDirection()
    {
        if (_agent.remainingDistance > _agent.stoppingDistance * 2f)
            return (_agent.steeringTarget - transform.position).normalized;
        else
            return (_currentTarget.transform.position - transform.position).normalized;
    }

    private Vector3 GetMoveDirection()
    {
        return (_currentTarget.transform.position - transform.position).normalized;
    }

    private float GetAngle(Vector3 direction)
    {
        return Vector3.Angle(transform.forward, direction);
    }

    private bool IsStopped()
    {
        if (_agent.remainingDistance > _agent.stoppingDistance)
            return false;

        return true;
    }
}
