using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyController : CreatureController
{
    [Header("Enemy")]
    [SerializeField] private float _walkSpeed = 4f;
    [SerializeField] private float _minStoppingDistance = 3f;
    [SerializeField] private float _maxStoppingDistance = 10f;
    [SerializeField] private EnemySight _sight;

    [Header("Wandering")]
    [SerializeField] private float _wanderingSpeed = 3f;
    [SerializeField] private float _wanderingRadius = 10f;
    [SerializeField] private float _wanderingStoppingDistance = 1f;

    [Header("Flex")]
    [SerializeField] private float _minFlexTime = 1f;
    [SerializeField] private float _maxFlexTime = 2f;
    [SerializeField] private float _chanceToFlex = 45f;

    public EnemySight Sight => _sight;

    private NavMeshAgent _agent;
    private AIState _state;

    private float _actionTimer;
    private float _timeBetweenActions = 0.5f;
    
    private float _wanderingStoppedTimer;
    private bool _wanderingPosSelected;

    private EnemyWeapon _weapon;
    private Vector3 _spawnedPosition;

    private float _flexTime;
    private Vector3 _flexDirection;

    private void Update()
    {
        WalkUpdate();
        FlexUpdate();
        ActionTimerUpdate();
        MoveUpdate(Time.deltaTime);
    }

    public void Init()
    {
        InitComponents();
        InitValues();
    }

    public void LoadProgress(EnemyData data)
    {
        transform.SetPositionAndRotation(data.position, data.rotation);
        _health.LoadProgress(data.health);
    }

    public void SetChaseState()
    {
        _state = AIState.Chase;

        ChangeSpeed();
        _health.BattleStart();
    }

    public bool CanTakeAction()
    {
        if (_actionTimer >= _timeBetweenActions)
            return true;

        return false;
    }

    public void ChangeStoppingDistance()
    {
        switch (_state)
        {
            case AIState.Waiting:
            case AIState.Wandering:
                _agent.stoppingDistance = _wanderingStoppingDistance;
                break;

            case AIState.Chase:
                if (_sight.SeeCurrentTarget)
                {
                    _agent.stoppingDistance = _weapon.Weapon.AttackDistance - 1f;
                    _agent.stoppingDistance = Mathf.Clamp(_agent.stoppingDistance, _minStoppingDistance, _maxStoppingDistance);
                }
                else
                {
                    _agent.stoppingDistance = _minStoppingDistance;
                }
                break;
        }
    }

    public bool IsStopped()
    {
        if (_agent.remainingDistance > _agent.stoppingDistance)
            return false;

        return true;
    }

    #region Overrides
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
        _weapon.Init();
    }
    #endregion /Overrides

    private void InitValues()
    {
        _spawnedPosition = transform.position;
        _agent.updateRotation = false;
        _agent.speed = _wanderingSpeed;
        _agent.stoppingDistance = _wanderingStoppingDistance;
    }

    private void ChangeSpeed()
    {
        if (Sight.Target == null)
        {
            _agent.speed = _wanderingSpeed;
            _agent.stoppingDistance = _wanderingStoppingDistance;
        }
        else
        {
            _agent.speed = _walkSpeed;
        }

        ChangeStoppingDistance();
    }

    private void WalkUpdate()
    {
        if (DisabledMoveInput)
        {
            _agent.isStopped = true;
            _animator.SetBool("IsRunning", false);
            _animator.SetFloat("WalkX", 0f);
            _animator.SetFloat("WalkZ", 0f);

            if (_state != AIState.Chase)
                _state = AIState.Waiting;

            return;
        }

        if (!CanTakeAction()) return;
        _agent.isStopped = false;
        ChangeSpeed();

        if (Sight.Target == null)
        {
            if (_wanderingStoppedTimer > 0f)
            {
                _animator.SetBool("IsRunning", false);
                _animator.SetFloat("WalkX", 0f);
                _animator.SetFloat("WalkZ", 0f);
                _wanderingStoppedTimer -= _timeBetweenActions;
                _state = AIState.Waiting;
                return;
            }

            if (!_wanderingPosSelected)
            {
                Vector3 targetPos = _spawnedPosition.RandomPointAroundXZ(_wanderingRadius);
                if (!_sight.IsGoalWithinReach(targetPos, _wanderingRadius, out targetPos)) return;

                _agent.destination = targetPos;
                _wanderingPosSelected = true;
                _animator.SetBool("IsRunning", true);
                _animator.SetFloat("WalkZ", 1f);
                _state = AIState.Wandering;
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
            _state = AIState.Chase;
            _agent.destination = Sight.Target.transform.position;

            FlexAction();

            float curSpeed = _agent.velocity.magnitude;
            if (curSpeed >= 0.05f)
            {
                _animator.SetBool("IsRunning", true);
                _animator.SetFloat("WalkZ", 1f);
            }
            else
            {
                _animator.SetBool("IsRunning", false);
                _animator.SetFloat("WalkX", 0f);
                _animator.SetFloat("WalkZ", 0f);
            }
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
            Vector3 lookDirection = _sight.GetLookDirection();
            _flexDirection += lookDirection;
            _flexDirection.Normalize();
        }

        _flexTime = Random.Range(_minFlexTime, _maxFlexTime);
    }

    private void FlexUpdate()
    {
        if (DisabledMoveInput) return;
        if (!_sight.SeeCurrentTarget) return;
        if (_flexTime <= 0f) return;

        _agent.Move(_agent.speed * Time.deltaTime * _flexDirection);
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

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Vector3 drawPos;
        if (Application.isPlaying)
        {
            drawPos = _spawnedPosition;
        }
        else
        {
            drawPos = transform.position;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(drawPos, _wanderingRadius);
    }
#endif
}
