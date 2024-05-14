using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyController : CreatureController
{
    [Header("Enemy")]
    [SerializeField] private float _viewingAngle = 90f;
    [Header("Speed")]
    [SerializeField] private float _walkSpeed = 4f;
    [SerializeField] private float _minStoppingDistance = 3f;
    [SerializeField] private float _maxStoppingDistance = 10f;
    [SerializeField] private float _minRotationSpeed = 100f;
    [SerializeField] private float _maxRotationSpeed = 500f;
    [Header("Wandering")]
    [SerializeField] private float _wanderingSpeed = 3f;
    [SerializeField] private float _wanderingRadius = 10f;
    [SerializeField] private float _wanderingStoppingDistance = 1f;
    [Header("Flex")]
    [SerializeField] private float _minFlexTime = 1f;
    [SerializeField] private float _maxFlexTime = 2f;
    [SerializeField] private float _chanceToFlex = 45f;

    private NavMeshAgent _agent;

    private float _actionTimer;
    private float _timeBetweenActions = 0.5f;
    
    private float _wanderingStoppedTimer;
    private bool _wanderingPosSelected;

    public CreatureHealth CurrentTarget => _currentTarget;
    private CreatureHealth _currentTarget;
    private List<CreatureHealth> _creatureInSight = new();

    public bool SeeCurrentTarget => _seeCurrentTarget;
    private bool _seeCurrentTarget;

    private EnemyWeapon _weapon;
    private Vector3 _spawnedPosition;

    private float _flexTime;
    private Vector3 _flexDirection;

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
        FlexUpdate();
        ActionTimerUpdate();
        MoveTimeUpdate(Time.deltaTime);
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

    public override void Push(Vector3 velocity) => Move(velocity);

    public override void Move(Vector3 velocity)
    {
        _agent.Move(velocity * Time.deltaTime);
    }

    public override void MoveToMovementDirection(float speed)
    {
        _agent.Move(speed * Time.deltaTime * _agent.velocity.normalized);
    }

    public override void MoveToLookDirection(float speed)
    {
        _agent.Move(speed * Time.deltaTime * transform.forward);
    }

    public override void Move(float xVelocity, float zVelocity)
    {
        Vector3 moveVel = new(xVelocity, _agent.velocity.y, zVelocity);
        _agent.Move(moveVel * Time.deltaTime);
    }

    protected override void InitComponents()
    {
        base.InitComponents();
        _agent = GetComponent<NavMeshAgent>();
        _weapon = GetComponent<EnemyWeapon>();
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
        ChangeSpeed();
    }

    private void ChangeSpeed()
    {
        if (_currentTarget == null)
        {
            _agent.speed = _wanderingSpeed;
            _agent.stoppingDistance = _wanderingStoppingDistance;
        }
        else
        {
            _agent.speed = _walkSpeed;
            if (_weapon == null)
            {
                _agent.stoppingDistance = _minStoppingDistance;
            }
            else
            {
                SetStoppingDistanceWeapon();
            }
        }
    }

    private void SetStoppingDistanceWeapon()
    {
        _agent.stoppingDistance = _weapon.Weapon.AttackDistance - 1f;
        _agent.stoppingDistance = Mathf.Clamp(_agent.stoppingDistance, _minStoppingDistance, _maxStoppingDistance);
    }

    private void SightUpdate()
    {
        if (!CanTakeAction()) return;
        if (_creatureInSight.Count <= 0) return;

        if (_currentTarget != null)
        {
            if (SeeTarget(_currentTarget))
            {
                _seeCurrentTarget = true;
                SetStoppingDistanceWeapon();
            }
            else
            {
                _seeCurrentTarget = false;
                _agent.stoppingDistance = _minStoppingDistance;
            }
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

                ChangeTarget(_creatureInSight[i]);
                break;
            }
        }
    }

    private bool IsGoalWithinReach(CreatureHealth enemy)
    {
        Vector3 direction = enemy.transform.position - transform.position;
        if (GetAngle(direction) > _viewingAngle)
            return false;

        if (!SeeTarget(enemy))
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

    private bool SeeTarget(CreatureHealth enemy)
    {
        return !Physics.Linecast(transform.position + Vector3.up, enemy.transform.position + Vector3.up, _groundLayer);
    }

    private void WalkUpdate()
    {
        if (DisabledMoveInput)
        {
            _agent.isStopped = true;
            return;
        }

        if (!CanTakeAction()) return;
        _agent.isStopped = false;
        ChangeSpeed();
        GroundCheckUpdate(Vector3.zero);

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
            FlexAction();
        }
    }

    private void FlexAction()
    {
        if (_flexTime > 0f)
        {
            _flexTime -= _timeBetweenActions;
            return;
        }

        float rand = Random.Range(0f, 100f);
        if (rand <= _chanceToFlex)
        {
            if (rand <= _chanceToFlex / 2f)
            {
                _flexDirection = Vector3.forward;
            }
            else
            {
                _flexDirection = -Vector3.forward;
            }
        }
        else
        {
            _flexDirection = Vector3.zero;
        }

        rand = Random.Range(0f, 100f);
        if (rand <= _chanceToFlex)
        {
            if (rand <= _chanceToFlex / 2f)
            {
                _flexDirection += Vector3.right;
            }
            else
            {
                _flexDirection -= Vector3.right;
            }
        }
        else
        {
            if (_flexDirection.z != 0f)
                _flexDirection = Vector3.zero;
        }

        if (_flexDirection != Vector3.zero)
        {
            Vector3 lookDirection = GetLookDirection();
            _flexDirection += lookDirection;
            _flexDirection.Normalize();
        }

        _flexTime = Random.Range(_minFlexTime, _maxFlexTime);
    }

    private void FlexUpdate()
    {
        if (DisabledMoveInput) return;
        if (!_seeCurrentTarget) return;
        if (_flexTime <= 0f) return;

        _agent.Move(_agent.speed * Time.deltaTime * _flexDirection);
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
        if (DisabledLookInput) return;
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
