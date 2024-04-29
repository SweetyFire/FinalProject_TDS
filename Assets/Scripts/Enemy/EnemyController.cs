using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : CreatureController
{
    [SerializeField] private float _viewingAngle = 90f;
    [SerializeField] private LayerMask _obstaclesMask;
    [Header("Speed")]
    [SerializeField] private float _walkSpeed = 4f;
    [SerializeField] private float _stoppingDistance = 3f;
    [SerializeField] private float _minRotationSpeed = 100f;
    [SerializeField] private float _maxRotationSpeed = 500f;
    [Header("Wandering")]
    [SerializeField] private float _wanderingSpeed = 3f;
    [SerializeField] private float _wanderingRadius = 10f;
    [SerializeField] private float _wanderingStoppingDistance = 1f;

    private NavMeshAgent _agent;

    private float _actionTimer;
    private float _timeBetweenActions = 0.5f;
    
    private float _wanderingStoppedTimer;
    private bool _wanderingPosSelected;

    private CreatureHealth _currentTarget;
    private List<CreatureHealth> _creatureInSight = new();

    private Vector3 _spawnedPosition;

    private void Awake()
    {
        InitComponents();
        InitValues();
    }

    private void Update()
    {
        SightUpdate();
        WalkUpdate();
        LookUpdate();
        ActionTimerUpdate();
    }

    public void OnCreatureEnterTrigger(Collider other)
    {
        if (_currentTarget != null) return;

        if (!other.TryGetComponent(out CreatureHealth creature)) return;
        _creatureInSight.Add(creature);

        if (creature is not PlayerHealth player) return;
        if (!IsGoalWithinReach(player)) return;

        NotifyAllies(player);
        ChangeTarget(player);
    }

    public void OnCreatureExitTrigger(Collider other)
    {
        if (_currentTarget != null) return;

        if (!other.TryGetComponent(out CreatureHealth creature)) return;
        if (!_creatureInSight.Contains(creature)) return;
        _creatureInSight.Remove(creature);
    }

    protected override void InitComponents()
    {
        base.InitComponents();
        _agent = GetComponent<NavMeshAgent>();
    }

    private void InitValues()
    {
        _spawnedPosition = transform.position;
        _agent.updateRotation = false;
        _agent.speed = _wanderingSpeed;
        _agent.stoppingDistance = _wanderingStoppingDistance;
    }

    private void NotifyAllies(CreatureHealth target)
    {
        if (_currentTarget != null) return;

        foreach (CreatureHealth creature in _creatureInSight)
        {
            if (creature is not EnemyHealth enemy) continue;
            enemy.Controller.ChangeTarget(target);
        }
    }

    private void ChangeTarget(CreatureHealth creature)
    {
        _currentTarget = creature;
        if (_currentTarget != null)
        {
            _agent.speed = _walkSpeed;
            _agent.stoppingDistance = _stoppingDistance;
        }
        else
        {
            _agent.speed = _wanderingSpeed;
            _agent.stoppingDistance = _wanderingStoppingDistance;
        }
    }

    private void SightUpdate()
    {
        if (!CanTakeAction()) return;
        if (_currentTarget != null) return;
        if (_creatureInSight.Count <= 0) return;

        foreach (CreatureHealth enemy in _creatureInSight)
        {
            if (enemy is EnemyHealth) continue;
            if (!IsGoalWithinReach(enemy)) continue;

            ChangeTarget(enemy);
            break;
        }
    }

    private bool IsGoalWithinReach(CreatureHealth enemy)
    {
        Vector3 direction = enemy.transform.position - transform.position;
        if (GetAngle(direction) > _viewingAngle)
            return false;

        if (Physics.Linecast(transform.position + Vector3.up, enemy.transform.position + Vector3.up, _obstaclesMask))
            return false;

        return true;
    }

    private bool IsGoalWithinReach(Vector3 position, float maxDistance, out Vector3 navMeshPosition)
    {
        navMeshPosition = position;
        NavMesh.SamplePosition(position, out NavMeshHit hit, maxDistance, NavMesh.AllAreas);

        NavMeshPath path = new();
        if (!_agent.CalculatePath(hit.position, path))
            return false;

        navMeshPosition = hit.position;
        return true;
    }

    private void WalkUpdate()
    {
        if (!CanTakeAction()) return;

        if (_currentTarget == null)
        {
            if (_wanderingStoppedTimer > 0f)
            {
                _wanderingStoppedTimer -= _timeBetweenActions;
                return;
            }

            if (!_wanderingPosSelected)
            {
                Vector3 targetPos = _spawnedPosition.RandomPointAroundXZ(_wanderingRadius);
                if (!IsGoalWithinReach(targetPos, _wanderingRadius, out targetPos)) return;

                _agent.destination = targetPos;
                _wanderingPosSelected = true;
            }
            else
            {
                if (_agent.remainingDistance <= _agent.stoppingDistance)
                {
                    _wanderingPosSelected = false;
                    _wanderingStoppedTimer = Random.Range(1f, 3f);
                }
            }
        }
        else
        {
            _agent.destination = _currentTarget.transform.position;
        }
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
        Vector3 steeringTargetDirection = (_agent.steeringTarget - transform.position).normalized;

        if (_currentTarget == null)
            return steeringTargetDirection;

        if (_agent.remainingDistance > _agent.stoppingDistance * 2f)
            return steeringTargetDirection;
        else
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
