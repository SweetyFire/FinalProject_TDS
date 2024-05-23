using System;
using UnityEngine;
using UnityEngine.Events;

public class ToggleObject : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] private bool _enabled;
    [SerializeField] private bool _correctValue;

    public bool Enabled => _enabled;
    public bool CorrectValue => _correctValue;
    public bool Deactivated { get; private set; }

    [Header("Events")]
    public UnityEvent<ToggleObject> onToggle;
    public UnityEvent onTurnedOn;
    public UnityEvent onTurnedOff;

    public Action OnDeactivated;

    private Collider _collider;

    private void Awake()
    {
        InitComponents();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out PlayerController player)) return;
        player.OnTriggerObjectEnter(this);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out PlayerController player)) return;
        player.OnTriggerObjectExit(this);
    }

    public void Init()
    {
        InitComponents();
    }

    public void Toggle()
    {
        Toggle(!_enabled);
    }

    public void Toggle(bool enable)
    {
        if (_enabled == enable) return;

        _enabled = enable;
        if (_enabled)
        {
            onTurnedOn?.Invoke();
        }
        else
        {
            onTurnedOff?.Invoke();
        }

        onToggle?.Invoke(this);
    }

    public void Deactivate()
    {
        Deactivated = true;
        _collider.enabled = false;
        OnDeactivated?.Invoke();
    }

    private void InitComponents()
    {
        if (_collider == null)
            _collider = GetComponent<Collider>();
    }
}
