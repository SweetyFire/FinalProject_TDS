using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1)]
public class InputManager : MonoBehaviour
{
    public InputManager Instance { get; private set; }
    public int NumberOfPlayers { get; private set; } = 1;

    private PlayerInputManager _manager;

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

        InitComponents();
    }

    private void InitComponents()
    {
        _manager = GetComponent<PlayerInputManager>();
    }
}
