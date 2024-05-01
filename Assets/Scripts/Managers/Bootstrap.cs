using UnityEngine;

[DefaultExecutionOrder(-1)]
public class Bootstrap : MonoBehaviour
{
    private GameManager _gameManager;
    private SoundManager _soundManager;

    private void Awake()
    {
        InitComponents();

        _gameManager.Init();
        _soundManager.Init();
    }

    private void InitComponents()
    {
        _gameManager = GetComponentInChildren<GameManager>();
        _soundManager = GetComponentInChildren<SoundManager>();
    }
}
