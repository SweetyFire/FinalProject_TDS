using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _walkSpeed = 150f;

    private Rigidbody _rb;
    private Vector3 _moveVelocity;

    private void Awake()
    {
        InitComponents();
    }

    private void FixedUpdate()
    {
        Vector3 velocity = _walkSpeed * Time.fixedDeltaTime * _moveVelocity;
        velocity.y = _rb.velocity.y;
        _rb.velocity = velocity;
    }

    private void InitComponents()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();
        _moveVelocity = new(input.x, 0f, input.y);
        _moveVelocity = Quaternion.Euler(0f, -45f, 0f) * _moveVelocity;
    }
}
