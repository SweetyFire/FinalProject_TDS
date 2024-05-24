using System.Collections.Generic;
using UnityEngine;

public abstract class CreatureController : MonoBehaviour
{
    [Header("Creature")]
    [SerializeField] protected LayerMask _groundLayer;
    [SerializeField] protected Animator _animator;
    [SerializeField] protected Transform _center;
    [SerializeField] protected GroundChecker _groundChecker;

    [Header("Sound")]
    [SerializeField] protected AudioSource _footstepAudioSource;
    [SerializeField] protected List<AudioClip> _footstepSounds = new();

    public float Height
    {
        get
        {
            float minValue = _collider.radius * 2f;
            if (_collider.height < minValue)
                return minValue;
            else
                return _collider.height;
        }
    }
    public float HalfHeight
    {
        get
        {
            float minValue = _collider.radius * 2f;
            float curValue = _collider.height / 2f;
            if (curValue < minValue)
                return minValue;
            else
                return curValue;
        }
    }
    public Vector3 CenterPosition => _center.position;
    public Transform Center => _center;

    public bool IsGrounded => _groundChecker.IsGrounded;
    public bool DisabledMoveInput => _disabledMoveInput || _isStunned || !_health.IsAlive || !GameLoader.Instance.GameLoaded || GameManager.GamePaused;
    public bool DisabledLookInput => _disabledLookInput || _isStunned || !_health.IsAlive || !GameLoader.Instance.GameLoaded || GameManager.GamePaused;
    public int Team => _health.Team;
    public CreatureHealth Health => _health;
    public bool IsStunned => _isStunned;

    protected float HalfColliderRadius => _collider.radius / 1.5f;

    protected Rigidbody _rb;
    protected CapsuleCollider _collider;
    protected CreatureHealth _health;

    private bool _disabledMoveInput;
    private bool _disabledLookInput;

    public bool PushMove => _pushTimer != -1f || _pushDistance != -1f;

    private VectorDirection _pushDirection;
    private Vector3 _pushVelocity;
    private float _pushSpeed;
    private float _pushTimer = -1f;
    private float _pushDistance = -1f;
    private bool _enableMoveAfterPush;
    private MTweenObject _pushAction = new();
    private bool _isStunned;

    protected virtual void InitComponents()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
        _health = GetComponent<CreatureHealth>();
        _health.Init();
    }

    public void EnableMove()
    {
        if (_isStunned) return;
        _disabledMoveInput = false;
    }

    public void DisableMove()
    {
        if (_disabledMoveInput) return;

        _pushAction.Reset();
        CompletePushByTime();
        CompletePushByDistance();

        _disabledMoveInput = true;
    }

    public void EnableLook()
    {
        if (_isStunned) return;
        _disabledLookInput = false;
    }

    public void DisableLook()
    {
        _disabledLookInput = true;
    }

    public virtual void EnablePhysics()
    {
        _collider.enabled = true;
    }

    public virtual void DisablePhysics()
    {
        _collider.enabled = false;
    }

    public void PlayFootsteps()
    {
        if (!IsGrounded) return;

        if (_footstepAudioSource.IsPlaying())
            if (_footstepAudioSource.GetTimePercent() < 0.5f) return;

        _footstepAudioSource.Stop();
        _footstepAudioSource.clip = _footstepSounds.GetRandom();
        _footstepAudioSource.SetRandomPitchAndVolume(0.9f, 1.1f, 0.5f, 0.6f);
        _footstepAudioSource.Play();
    }

    public MTweenObject MoveTime(VectorDirection direction, float speed, float time, bool enableMoveAfter = true)
    {
        DisableMove();

        _pushTimer = time;
        _pushSpeed = speed;
        _pushDirection = direction;
        _pushVelocity = Vector3.zero;
        _enableMoveAfterPush = enableMoveAfter;
        return _pushAction;
    }

    public MTweenObject MoveTime(Vector3 velocity, float time, bool enableMoveAfter = true)
    {
        DisableMove();

        _pushTimer = time;
        _pushVelocity = velocity;
        _pushSpeed = _pushVelocity.magnitude;
        _pushDirection = VectorDirection.Zero;
        _enableMoveAfterPush = enableMoveAfter;
        return _pushAction;
    }

    public void CompletePushByTime()
    {
        if (_pushTimer <= -1f) return;

        if (_enableMoveAfterPush)
            EnableMove();

        _pushTimer = -1f;
        _pushAction.Complete();
        _pushVelocity = Vector3.zero;
        _enableMoveAfterPush = false;
        _pushDirection = VectorDirection.Zero;
    }

    public MTweenObject MoveDistance(VectorDirection direction, float speed, float distance, bool enableMoveAfter = true)
    {
        DisableMove();

        _pushSpeed = speed;
        _pushDistance = distance;
        _pushDirection = direction;
        _pushVelocity = Vector3.zero;
        _enableMoveAfterPush = enableMoveAfter;
        return _pushAction;
    }

    public MTweenObject MoveDistance(Vector3 velocity, float distance, bool enableMoveAfter = true)
    {
        DisableMove();

        _pushVelocity = velocity;
        _pushDistance = distance;
        _pushSpeed = _pushVelocity.magnitude;
        _pushDirection = VectorDirection.Zero;
        _enableMoveAfterPush = enableMoveAfter;
        return _pushAction;
    }

    public void CompletePushByDistance()
    {
        if (_pushDistance <= -1f) return;

        if (_enableMoveAfterPush)
            EnableMove();

        _pushDistance = -1f;
        _pushAction.Complete();
        _enableMoveAfterPush = false;
        _pushVelocity = Vector3.zero;
        _pushDirection = VectorDirection.Zero;
    }

    public Vector3 CalculatePushDirection(VectorDirection direction)
    {
        return direction switch
        {
            VectorDirection.Forward => transform.forward,
            VectorDirection.Backward => -transform.forward,
            VectorDirection.Right => transform.right,
            VectorDirection.Left => -transform.right,
            VectorDirection.Up => transform.up,
            VectorDirection.Down => -transform.up,
            _ => Vector3.zero,
        };
    }

    protected void MoveToPushDirection()
    {
        switch (_pushDirection)
        {
            case VectorDirection.Forward:
                Move(transform.forward * _pushSpeed);
                break;

            case VectorDirection.Backward:
                Move(-transform.forward * _pushSpeed);
                break;

            case VectorDirection.Right:
                Move(transform.right * _pushSpeed);
                break;

            case VectorDirection.Left:
                Move(-transform.right * _pushSpeed);
                break;

            case VectorDirection.Up:
                Move(transform.up * _pushSpeed);
                break;

            case VectorDirection.Down:
                Move(-transform.up * _pushSpeed);
                break;

            case VectorDirection.Movement:
                MoveToMovementDirection(_pushSpeed);
                break;

            case VectorDirection.Look:
                MoveToLookDirection(_pushSpeed);
                break;
        }
    }

    private Vector3 GetPushVelocity()
    {
        if (_pushVelocity == Vector3.zero)
        {
            Vector3 velocity = _pushDirection switch
            {
                VectorDirection.Look or VectorDirection.Forward => transform.forward,
                VectorDirection.Backward => -transform.forward,
                VectorDirection.Right => transform.right,
                VectorDirection.Left => -transform.right,
                VectorDirection.Up => transform.up,
                VectorDirection.Down => -transform.up,
                VectorDirection.Movement => GetMoveDirection(),
                _ => Vector3.zero,
            };

            velocity *= _pushSpeed;
            return velocity;
        }
        else
            return _pushVelocity;
    }

    public abstract void Push(Vector3 velocity);
    public abstract void Move(Vector3 velocity);
    public abstract void Move(float xVelocity, float zVelocity);
    public abstract void MoveToMovementDirection(float speed);
    public abstract void MoveToLookDirection(float speed);
    protected abstract Vector3 GetMoveDirection();

    protected void MoveUpdate(float deltaTime)
    {
        if (_pushTimer > 0f)
        {
            PushByTime(deltaTime);
        }
        else
        {
            CompletePushByTime();
        }

        if (_pushDistance > 0f)
        {
            PushByDistance(deltaTime);
        }
        else
        {
            CompletePushByDistance();
        }
    }

    private void PushByDistance(float deltaTime)
    {
        Move(GetPushVelocity());
        _pushDistance -= _pushSpeed * deltaTime;
        _pushAction.Update();
    }

    private void PushByTime(float deltaTime)
    {
        Move(GetPushVelocity());
        _pushTimer -= deltaTime;
        _pushAction.Update();
    }
}
