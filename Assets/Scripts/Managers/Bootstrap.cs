using UnityEngine;

[DefaultExecutionOrder(-1)]
public class Bootstrap : MonoBehaviour
{
    private UIManager _uiManager;
    private GameManager _gameManager;
    private SoundManager _soundManager;

    private void Awake()
    {
        InitComponents();

        _uiManager.Init();
        _gameManager.Init();
        _soundManager.Init();
    }

    private void InitComponents()
    {
        _uiManager = GetComponentInChildren<UIManager>();
        _gameManager = GetComponentInChildren<GameManager>();
        _soundManager = GetComponentInChildren<SoundManager>();
    }
}
