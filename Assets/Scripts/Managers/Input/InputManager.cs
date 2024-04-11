using System;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public event Action<Vector2> OnMove;
    public event Action OnShoot;

    private PlayerControls _controls;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }

        _controls = new();
        InitEvents();
    }

    private void OnEnable()
    {
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Disable();
    }

    private void InitEvents()
    {
        _controls.Player.Move.performed += ctx => OnMove?.Invoke(ctx.ReadValue<Vector2>());
        _controls.Player.Move.canceled += ctx => OnMove?.Invoke(ctx.ReadValue<Vector2>());

        _controls.Player.Shoot.canceled += ctx => OnShoot?.Invoke();
    }
}
