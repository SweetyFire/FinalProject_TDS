using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : CreatureController
{
    [SerializeField] private float _walkSpeed = 5f;
    [SerializeField] private float _rotationSpeed = 500f;
    [SerializeField] private float _maxSlopeAngle = 45f;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private Camera _camera;

    private Rigidbody _rb;
    private Vector3 _moveDirection;
    private Vector2 _lookDirection;
    private Vector2 _mousePosition;
    private float _cameraAngle;
    private PlayerWeapon _weapon;

    private bool _isGrounded;
    private RaycastHit _groundHit;
    private float _groundCheckOffset = 0.1f;

    private void Awake()
    {
        InitComponents();
        SetCameraAngle();
    }

    private void Update()
    {
        LookUpdate();
        GroundCheckUpdate();
    }

    private void FixedUpdate()
    {
        MoveFixedUpdate();
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();
        _moveDirection = new(input.x, 0f, input.y);
        _moveDirection = _moveDirection.RotateTo(0f, _cameraAngle, 0f);
    }

    public void OnLook(InputAction.CallbackContext ctx)
    {
        _lookDirection = ctx.ReadValue<Vector2>();
    }

    public void OnMousePositionChanged(InputAction.CallbackContext ctx)
    {
        _mousePosition = ctx.ReadValue<Vector2>();
    }

    protected override void InitComponents()
    {
        base.InitComponents();
        _rb = GetComponent<Rigidbody>();
        _weapon = GetComponent<PlayerWeapon>();
    }

    private void MoveFixedUpdate()
    {
        float slopeAngle = GetSlopeAngle();
        _rb.useGravity = true;

        Vector3 velocity;
        if (slopeAngle > 0f)
        {
            if (slopeAngle > _maxSlopeAngle)
            {
                velocity = Vector3.zero;
                velocity.y = _rb.velocity.y;
            }
            else
            {
                velocity = GetSlopeMoveDirection() * _walkSpeed;
                _rb.useGravity = false;
            }
        }
        else
        {
            velocity = _moveDirection * _walkSpeed;
            velocity.y = _rb.velocity.y;
        }

        _rb.velocity = velocity;
        SpeedControl();
    }

    private bool OnSlope()
    {
        if (_isGrounded)
        {
            float angle = GetSlopeAngle();
            return angle != 0f && angle <= _maxSlopeAngle;
        }

        return false;
    }

    private void SpeedControl()
    {
        Vector3 limitedVel = _rb.velocity;
        if (_isGrounded)
        {
            if (limitedVel.magnitude > _walkSpeed)
            {
                limitedVel = limitedVel.normalized * _walkSpeed;

                if (!OnSlope())
                    limitedVel.y = _rb.velocity.y;
            }
        }

        _rb.velocity = limitedVel;
    }

    private void GroundCheckUpdate()
    {
        float groundCheckRadius = _collider.radius / 1.5f;
        float groundCheckDistance = groundCheckRadius + _groundCheckOffset;
        Vector3 castPos = (_groundCheckOffset + groundCheckRadius) * Vector3.up + transform.position;
        castPos += _moveDirection * (groundCheckRadius / 2f);
        _isGrounded = Physics.SphereCast(castPos, groundCheckRadius, Vector3.down, out _groundHit, groundCheckDistance, _groundLayer);
    }

    private void SetCameraAngle()
    {
        _cameraAngle = _camera.transform.localRotation.eulerAngles.y;
    }

    private void LookUpdate()
    {
        Quaternion rotation;
        Vector3 lookVelocity;

        if (_weapon.IsAttacking)
        {
            if (_playerInput.currentControlScheme == "Keyboard")
            {
                Ray mouseRay = _camera.ScreenPointToRay(_mousePosition);
                if (Physics.SphereCast(mouseRay, 0.1f, out RaycastHit hit))
                {
                    Vector3 mouseInWorld = hit.point;
                    lookVelocity = (mouseInWorld - transform.position).normalized;
                    lookVelocity = new(lookVelocity.x, 0f, lookVelocity.z);
                }
                else
                {
                    Vector2 onScreenPos = _camera.WorldToScreenPoint(transform.position);
                    lookVelocity = (_mousePosition - onScreenPos).normalized;
                    lookVelocity = new(lookVelocity.x, 0f, lookVelocity.y);
                    lookVelocity = lookVelocity.RotateTo(0f, _cameraAngle, 0f);
                }

            }
            else
            {
                if (_lookDirection == Vector2.zero) return;
                lookVelocity = new(_lookDirection.x, 0f, _lookDirection.y);
                lookVelocity = lookVelocity.RotateTo(0f, _cameraAngle, 0f);
            }
        }
        else
        {
            if (_moveDirection == Vector3.zero) return;
            lookVelocity = _moveDirection;
        }
        
        rotation = Quaternion.LookRotation(lookVelocity, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, _rotationSpeed * Time.deltaTime);
    }

    private float GetSlopeAngle()
    {
        return Vector3.Angle(Vector3.up, _groundHit.normal);
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(_moveDirection, _groundHit.normal).normalized;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        CapsuleCollider col;
        if (_collider == null)
            col = GetComponent<CapsuleCollider>();
        else
            col = _collider;

        float groundCheckRadius = col.radius / 1.5f;
        float groundCheckDistance = groundCheckRadius + _groundCheckOffset;
        Vector3 castPos = (_groundCheckOffset + groundCheckRadius) * Vector3.up + transform.position;
        castPos += _moveDirection * (groundCheckRadius / 2f);

        if (Physics.SphereCast(castPos, groundCheckRadius, Vector3.down, out RaycastHit hit, groundCheckDistance, _groundLayer))
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(castPos, Vector3.down * hit.distance);
            Gizmos.DrawWireSphere(castPos + Vector3.down * hit.distance, groundCheckRadius);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(castPos, groundCheckDistance * Vector3.down);
            Gizmos.DrawWireSphere(castPos + groundCheckDistance * Vector3.down, groundCheckRadius);
        }
    }
#endif
}
