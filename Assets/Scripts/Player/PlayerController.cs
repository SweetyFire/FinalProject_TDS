using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : CreatureController
{
    [SerializeField] private float _walkSpeed = 150f;
    [SerializeField] private float _rotationSpeed = 500f;
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private Camera _camera;

    private Rigidbody _rb;
    private Vector3 _moveVelocity;
    private Vector2 _lookInput;
    private Vector2 _mousePosition;
    private float _cameraAngle;
    private PlayerWeapon _weapon;

    private void Awake()
    {
        InitComponents();
        SetCameraAngle();
    }

    private void Update()
    {
        LookUpdate();
    }

    private void FixedUpdate()
    {
        Vector3 velocity = _walkSpeed * Time.fixedDeltaTime * _moveVelocity;
        velocity.y = _rb.velocity.y;
        _rb.velocity = velocity;
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();
        _moveVelocity = new(input.x, 0f, input.y);
        _moveVelocity = _moveVelocity.RotateTo(0f, _cameraAngle, 0f);
    }

    public void OnLook(InputAction.CallbackContext ctx)
    {
        _lookInput = ctx.ReadValue<Vector2>();
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
                if (_lookInput == Vector2.zero) return;
                lookVelocity = new(_lookInput.x, 0f, _lookInput.y);
                lookVelocity = lookVelocity.RotateTo(0f, _cameraAngle, 0f);
            }
        }
        else
        {
            if (_moveVelocity == Vector3.zero) return;
            lookVelocity = _moveVelocity;
        }
        
        rotation = Quaternion.LookRotation(lookVelocity, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, _rotationSpeed * Time.deltaTime);
    }
}
