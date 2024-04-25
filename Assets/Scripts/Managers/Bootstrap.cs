using UnityEngine;

[DefaultExecutionOrder(-1)]
public class Bootstrap : MonoBehaviour
{
    [SerializeField] private UIManager _uiManager;

    private void Awake()
    {
        _uiManager.Init();
    }
}
