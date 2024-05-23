using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(-1)]
public class Bootstrap : MonoBehaviour
{
    [SerializeField] private List<InitializableBehavior> _initializables = new();
    public UnityEvent OnAwake;
    public UnityEvent OnStart;

    private void Awake()
    {
        for (int i = 0; i < _initializables.Count; i++)
        {
            _initializables[i].Initialize();
        }

        OnAwake?.Invoke();
    }

    private void Start()
    {
        OnStart?.Invoke();
    }
}
