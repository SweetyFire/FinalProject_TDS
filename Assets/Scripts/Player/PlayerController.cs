using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _walkSpeed = 150f;

    private Rigidbody _rb;
    private Vector3 _moveVelocity;

    private void Awake()
    {
        InitComponents();
        InitInput();
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

    private void InitInput()
    {
        InputManager.Instance.OnMove += OnMove;
    }

    private void OnMove(Vector2 input)
    {
        _moveVelocity = new(input.x, 0f, input.y);
    }
}
