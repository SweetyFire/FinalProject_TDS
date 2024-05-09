using UnityEngine;
using UnityEngine.Events;

public abstract class Ability : MonoBehaviour
{
    [Header("Ability")]
    [SerializeField] protected float _cooldown = 1f;
    [SerializeField] protected bool _enableMoveInput;
    [SerializeField] protected bool _enableLookInput;

    public float Cooldown => _cooldown;
    public float RemainingCooldownTime => _cooldownTime;
    public bool EnableMoveInput => _enableMoveInput;
    public bool EnableLookInput => _enableLookInput;

    public UnityEvent<Ability> OnStarted;
    public UnityEvent<Ability> OnCompleted;

    protected AbilityCaster _owner;
    private float _cooldownTime;

    protected virtual void Update()
    {
        CooldownUpdate();
    }

    public void Init(AbilityCaster caster)
    {
        _owner = caster;
    }

    public void Activate()
    {
        if (_cooldownTime > 0f) return;

        _cooldownTime = _cooldown;
        OnStarted?.Invoke(this);
        ActivateAbility();
    }

    protected abstract void ActivateAbility();

    private void CooldownUpdate()
    {
        if (_cooldownTime > 0f)
        {
            _cooldownTime -= Time.deltaTime;
        }
    }
}
