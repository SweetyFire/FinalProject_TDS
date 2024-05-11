using UnityEngine;

public abstract class CreatureController : MonoBehaviour
{
    [Header("Creature")]
    [SerializeField] protected LayerMask _groundLayer;

    public float Height => _collider.height;
    public Vector3 Center => transform.position + Vector3.up;

    public bool IsGrounded => _isGrounded;
    public bool DisabledMoveInput => _disabledMoveInput || _isStunned;
    public bool DisabledLookInput => _disabledLookInput || _isStunned;
    public int Team => _health.Team;
    public CreatureHealth Health => _health;
    public bool IsStunned => _isStunned;

    protected float HalfColliderRadius => _collider.radius / 1.5f;
    protected float GroundCheckDistance => HalfColliderRadius + _groundCheckOffset;

    protected Rigidbody _rb;
    protected CapsuleCollider _collider;
    protected CreatureHealth _health;

    protected RaycastHit _groundHit;
    protected float _groundCheckOffset = 0.1f;

    private bool _isGrounded;
    private bool _disabledMoveInput;
    private bool _disabledLookInput;

    private Vector3 _pushVelocity;
    private float _pushTimer = -1f;
    private bool _isStunned;

    protected virtual void InitComponents()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
        _health = GetComponent<CreatureHealth>();
    }

    public void Move(Vector3 velocity, float time)
    {
        _pushVelocity = velocity;
        _pushTimer = time;
    }

    public abstract void Push(Vector3 velocity);
    public abstract void Move(Vector3 velocity);
    public abstract void Move(float xVelocity, float zVelocity);
    public abstract void MoveToMovementDirection(float speed);
    public abstract void MoveToLookDirection(float speed);

    protected void MoveTimeUpdate(float deltaTime)
    {
        if (_pushTimer <= -1f) return;
        if (_pushTimer > 0f)
        {
            DisableMove();
            _pushTimer -= deltaTime;
            Move(_pushVelocity);
            return;
        }

        _pushTimer = -1f;
        EnableMove();
    }

    protected void GroundCheckUpdate(Vector3 moveDirection)
    {
        Vector3 castPos = (_groundCheckOffset + HalfColliderRadius) * Vector3.up + transform.position;
        castPos += moveDirection * (HalfColliderRadius / 2f);
        _isGrounded = Physics.SphereCast(castPos, HalfColliderRadius, Vector3.down, out _groundHit, GroundCheckDistance, _groundLayer);
    }

    public void EnableMove()
    {
        if (_isStunned) return;
        _disabledMoveInput = false;
    }

    public void DisableMove()
    {
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
}
