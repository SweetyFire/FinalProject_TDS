using System;
using UnityEngine;

public abstract class CreatureController : MonoBehaviour
{
    [Header("Creature")]
    [SerializeField] protected LayerMask _groundLayer;

    public float Height => _collider.height;
    public bool IsGrounded => _isGrounded;
    public bool DisabledMoveInput => _disabledMoveInput;
    public bool DisabledLookInput => _disabledLookInput;

    protected float HalfColliderRadius => _collider.radius / 1.5f;
    protected float GroundCheckDistance => HalfColliderRadius + _groundCheckOffset;

    protected Rigidbody _rb;
    protected CapsuleCollider _collider;

    protected RaycastHit _groundHit;
    protected float _groundCheckOffset = 0.1f;

    private bool _isGrounded;
    private bool _disabledMoveInput;
    private bool _disabledLookInput;

    protected virtual void InitComponents()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
    }

    public abstract void Push(Vector3 velocity);
    public abstract void Move(Vector3 velocity);
    public abstract void Move(float xVelocity, float zVelocity);
    public abstract void MoveToMovementDirection(float speed);
    public abstract void MoveToLookDirection(float speed);

    protected void GroundCheckUpdate(Vector3 moveDirection)
    {
        Vector3 castPos = (_groundCheckOffset + HalfColliderRadius) * Vector3.up + transform.position;
        castPos += moveDirection * (HalfColliderRadius / 2f);
        _isGrounded = Physics.SphereCast(castPos, HalfColliderRadius, Vector3.down, out _groundHit, GroundCheckDistance, _groundLayer);
    }

    protected void EnableMove()
    {
        _disabledMoveInput = false;
    }

    protected void DisableMove()
    {
        _disabledMoveInput = true;
    }

    protected void EnableLook()
    {
        _disabledLookInput = false;
    }

    protected void DisableLook()
    {
        _disabledLookInput = true;
    }
}
