using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
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
        _moveVelocity = Quaternion.Euler(0f, _cameraAngle, 0f) * _moveVelocity;
    }

    public void OnLook(InputAction.CallbackContext ctx)
    {
        _lookInput = ctx.ReadValue<Vector2>();
    }

    public void OnMousePositionChanged(InputAction.CallbackContext ctx)
    {
        _mousePosition = ctx.ReadValue<Vector2>();
    }

    private void InitComponents()
    {
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

        if (_weapon.IsAttacking)
        {
            if (_playerInput.currentControlScheme == "Keyboard")
            {
                Vector2 screenPos = _camera.WorldToScreenPoint(transform.position);
                Vector2 direction = (_mousePosition - screenPos).normalized;
                Vector3 velocity = new(direction.x, 0f, direction.y);
                velocity = Quaternion.Euler(0f, _cameraAngle, 0f) * velocity;
                rotation = Quaternion.LookRotation(velocity, Vector3.up);
            }
            else
            {
                if (_lookInput == Vector2.zero) return;

                Vector3 lookVelocity = new(_lookInput.x, 0f, _lookInput.y);
                rotation = Quaternion.LookRotation(lookVelocity, Vector3.up);
            }
        }
        else
        {
            if (_moveVelocity == Vector3.zero) return;
            rotation = Quaternion.LookRotation(_moveVelocity, Vector3.up);
        }

        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, _rotationSpeed * Time.deltaTime);
    }
}
