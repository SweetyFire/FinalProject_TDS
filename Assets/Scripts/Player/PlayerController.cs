using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : CreatureController
{
    [Header("Player")]
    [SerializeField] private float _walkSpeed = 5f;
    [SerializeField] private float _rotationSpeed = 500f;
    [SerializeField] private float _maxSlopeAngle = 45f;
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private Camera _camera;
    [SerializeField] private LayerMask _lookMask;
    [SerializeField] private CameraFollow _cameraFollow;

    public Camera Camera => _camera;

    [Header("Stairs")]
    [SerializeField] private float _stepHeight = 0.3f;
    [SerializeField] private float _stepSmooth = 0.1f;

    [Header("UI")]
    [SerializeField] private GameplayUI _gameplayUI;
    public GameplayUI GameplayUI => _gameplayUI;

    private Vector3 _moveDirection;
    private Vector2 _lookDirection;
    private Vector2 _mousePosition;
    private float _cameraAngle;
    private PlayerWeapon _weapon;
    private AbilityCaster _abilityCaster;

    private RaycastHit _groundHit;

    private ToggleObject _currentToggleObject;
    private List<ToggleObject> _toggleObjects = new();

    private void Start()
    {
        if (_rb == null)
            throw new System.Exception("Please put the GameLoader into the scene and assign the player to it");
    }

    private void FixedUpdate()
    {
        MoveFixedUpdate();
        LookFixedUpdate();
        MoveUpdate(Time.fixedDeltaTime);
    }

    public void Init()
    {
        InitComponents();
        SetCameraAngle();
        _abilityCaster.Init(this, _weapon);
        _cameraFollow.InitAxis();
    }

    public void LoadProgress(PlayerData playerData)
    {
        transform.SetPositionAndRotation(playerData.position, playerData.rotation);
        _cameraFollow.transform.position = transform.position;
        _health.LoadProgress(playerData.health);
    }

    public void OnTriggerObjectEnter(ToggleObject toggleObject)
    {
        _currentToggleObject = toggleObject;
        toggleObject.OnDeactivated += OnTriggerObjectDeactivated;

        _toggleObjects.Add(toggleObject);
        _gameplayUI.InteractibleHint.SetFollow(_currentToggleObject.gameObject);
    }

    public void OnTriggerObjectExit(ToggleObject toggleObject)
    {
        if (!_toggleObjects.Contains(toggleObject)) return;

        toggleObject.OnDeactivated -= OnTriggerObjectDeactivated;
        _toggleObjects.Remove(toggleObject);

        if (_toggleObjects.Count > 0)
        {
            foreach (ToggleObject obj in  _toggleObjects)
            {
                if (!obj.Deactivated) return;
            }
        }

        _currentToggleObject = null;
        _gameplayUI.InteractibleHint.Disable();
    }

    #region Input
    public void OnMove(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();
        _moveDirection = new(input.x, 0f, input.y);
        
        if (DisabledMoveInput || _moveDirection == Vector3.zero)
        {
            _animator.SetBool("IsRunning", false);
            _animator.SetFloat("WalkX", 0f);
            _animator.SetFloat("WalkZ", 0f);
        }
        else if (!DisabledMoveInput)
        {
            _animator.SetBool("IsRunning", true);
            _animator.SetFloat("WalkX", 0f);
            _animator.SetFloat("WalkZ", Mathf.Clamp01(input.magnitude));
        }

        _moveDirection = _moveDirection.RotateTo(0f, _cameraAngle, 0f);

        if (ctx.canceled) return;
        _abilityCaster.InterruptAbilityCasting();
    }

    public void OnLook(InputAction.CallbackContext ctx)
    {
        _lookDirection = ctx.ReadValue<Vector2>();
    }

    public void OnMousePositionChanged(InputAction.CallbackContext ctx)
    {
        _mousePosition = ctx.ReadValue<Vector2>();
    }

    public void OnFirstAbility(InputAction.CallbackContext ctx)
    {
        if (!ctx.started) return;
        ActivateAbility(0);
    }

    public void OnSecondAbility(InputAction.CallbackContext ctx)
    {
        if (!ctx.started) return;
        ActivateAbility(1);
    }

    public void OnUltimateAbility(InputAction.CallbackContext ctx)
    {
        if (!ctx.started) return;
        ActivateAbility(2);
    }

    public void OnInteractible(InputAction.CallbackContext ctx)
    {
        if (!ctx.started) return;
        if (_currentToggleObject == null) return;

        _currentToggleObject.Toggle();
    }
    #endregion /Input

    #region Overrides
    public override void Push(Vector3 velocity)
    {
        _rb.AddForce(velocity, ForceMode.Impulse);
    }

    public override void Move(Vector3 velocity)
    {
        _rb.velocity = velocity;
    }

    public override void Move(float xVelocity, float zVelocity)
    {
        Vector3 moveVel = new(xVelocity, _rb.velocity.y, zVelocity);
        _rb.velocity = moveVel;
    }

    public override void MoveToMovementDirection(float speed)
    {
        if (_moveDirection != Vector3.zero)
        {
            Vector3 velocity = _moveDirection * speed;
            velocity.y = _rb.velocity.y;
            _rb.velocity = velocity;
        }
        else
        {
            MoveToLookDirection(speed);
        }
    }

    public override void MoveToLookDirection(float speed)
    {
        Vector3 velocity = transform.forward * speed;
        velocity.y = _rb.velocity.y;
        _rb.velocity = velocity;
    }

    public override void EnablePhysics()
    {
        base.EnablePhysics();
        _rb.isKinematic = false;
    }

    public override void DisablePhysics()
    {
        base.DisablePhysics();
        _rb.isKinematic = true;
    }

    protected override void InitComponents()
    {
        base.InitComponents();
        _weapon = GetComponent<PlayerWeapon>();
        _weapon.Init();
        _abilityCaster = GetComponent<AbilityCaster>();
    }

    #endregion /Overrides

    private void MoveFixedUpdate()
    {
        _rb.useGravity = true;

        Vector3 velocity;
        if (DisabledMoveInput)
        {
            if (PushMove) return;

            velocity = Vector3.zero;
            velocity.y = _rb.velocity.y;
            _rb.velocity = velocity;
            return;
        }

        float slopeAngle = GetSlopeAngle();
        if (IsGrounded && slopeAngle > 0f)
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

        if (IsGrounded && slopeAngle <= 5f)
            StepClimbFixedUpdate();
    }

    private void LookFixedUpdate()
    {
        if (DisabledLookInput) return;

        Quaternion rotation;
        Vector3 lookVelocity;

        // Rotate To Cursor or Stick Direction
        if (_weapon.IsAttacking || (_abilityCaster.IsCasting && !_abilityCaster.NotLookTargetWhileCast))
        {
            if (_playerInput.currentControlScheme == "Keyboard")
            {
                Ray mouseRay = _camera.ScreenPointToRay(_mousePosition);
                if (Physics.SphereCast(mouseRay, 0.1f, out RaycastHit hit, _camera.farClipPlane, _lookMask))
                {
                    Vector3 mouseInWorld = hit.point;
                    lookVelocity = (mouseInWorld - _rb.position).normalized;
                    lookVelocity = new(lookVelocity.x, 0f, lookVelocity.z);
                }
                else
                {
                    Vector2 onScreenPos = _camera.WorldToScreenPoint(_rb.position);
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

        // Rotate To Move Direction
        else
        {
            if (_moveDirection == Vector3.zero) return;
            lookVelocity = _moveDirection;
        }

        rotation = Quaternion.LookRotation(lookVelocity, Vector3.up);
        _rb.rotation = Quaternion.RotateTowards(_rb.rotation, rotation, _rotationSpeed);
    }

    private void StepClimbFixedUpdate()
    {
        if (_moveDirection == Vector3.zero) return;

        Vector3 upperCastPos = transform.position + Vector3.up * (_stepHeight + 0.1f);
        float castDistanceUpper = _collider.radius + 0.2f;

        if (IsLowerCast(0.1f))
        {
            if (!Physics.Raycast(upperCastPos, _moveDirection, castDistanceUpper, _groundLayer))
            {
                _rb.position += Vector3.up * _stepSmooth;
            }
        }
    }

    private void ActivateAbility(int index)
    {
        if (_abilityCaster.CanCasting(index))
        {
            if (_moveDirection != Vector3.zero) return;
        }

        _abilityCaster.ActivateAbility(index);
    }

    private bool OnSlope()
    {
        if (!IsGrounded) return false;

        float angle = GetSlopeAngle();
        return angle != 0f && angle <= _maxSlopeAngle;
    }

    private void SpeedControl()
    {
        Vector3 limitedVel = _rb.velocity;
        if (limitedVel.magnitude > _walkSpeed)
        {
            limitedVel = limitedVel.normalized * _walkSpeed;

            if (!OnSlope())
                limitedVel.y = _rb.velocity.y;
        }

        _rb.velocity = limitedVel;
    }

    private void SetCameraAngle()
    {
        _cameraAngle = _camera.transform.localRotation.eulerAngles.y;
    }

    private float GetSlopeAngle()
    {
        if (!Physics.Raycast(transform.position, Vector3.down, out _groundHit, _collider.radius, _groundLayer)) return 0f;

        if (!IsLowerCast(0.2f, 0.2f)) return 0f;
        return Vector3.Angle(Vector3.up, _groundHit.normal);
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(_moveDirection, _groundHit.normal).normalized;
    }

    private bool IsLowerCast(float distanceFromFloor)
    {
        return IsLowerCast(distanceFromFloor, 0.1f);
    }

    private bool IsLowerCast(float distanceFromFloor, float forwardDistance)
    {
        Vector3 lowerCastPos = transform.position + Vector3.up * distanceFromFloor;
        Vector3 direction = _moveDirection == Vector3.zero ? transform.forward : _moveDirection;
        float castDistanceLower = _collider.radius + forwardDistance;
        return Physics.Raycast(lowerCastPos, direction, castDistanceLower, _groundLayer);
    }

    private void OnTriggerObjectDeactivated()
    {
        OnTriggerObjectExit(_currentToggleObject);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        CapsuleCollider col;
        if (_collider == null)
            _collider = GetComponent<CapsuleCollider>();

        col = _collider;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit slopeHit, _collider.radius, _groundLayer))
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, Vector3.down * slopeHit.distance);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, Vector3.down * _collider.radius);
        }

        Vector3 lowerCastPos = transform.position + Vector3.up * 0.1f;
        Vector3 upperCastPos = lowerCastPos + Vector3.up * _stepHeight;
        Vector3 direction = _moveDirection == Vector3.zero ? transform.forward : _moveDirection;

        float castDistanceLower = col.radius + 0.1f;
        float castDistanceUpper = col.radius + 0.2f;
        if (Physics.Raycast(lowerCastPos, direction, castDistanceLower, _groundLayer))
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(lowerCastPos, direction * castDistanceLower);

            if (!Physics.Raycast(upperCastPos, direction, castDistanceUpper, _groundLayer))
            {
                Gizmos.color = Color.red;
            }
            else
            {
                Gizmos.color = Color.green;
            }

            Gizmos.DrawRay(upperCastPos, direction * castDistanceUpper);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(lowerCastPos, direction * castDistanceLower);
        }
    }
#endif
}
