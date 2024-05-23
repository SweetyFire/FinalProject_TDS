using UnityEngine;
using UnityEngine.Events;

public abstract class CreatureHealth : MonoBehaviour
{
    [Header("Creature")]
    [SerializeField] protected float _maxValue = 100f;
    [SerializeField] protected int _team;
    [SerializeField] protected Animator _animator;

    [Header("Particles")]
    [SerializeField] protected ParticleSystem _hurtParticles;

    public UnityEvent<CreatureHealth> OnDestroyed;

    protected float _value;

    public float MaxValue => _maxValue;
    public float Value => _value;

    public bool IsAlive => _isAlive;
    public int Team => _team;

    public CreatureController Controller => _controller;
    protected CreatureController _controller;

    protected bool _isAlive = true;
    protected bool _inBattle;

    public void Init()
    {
        InitComponents();
        InitValues();
    }

    public void LoadProgress(float health)
    {
        _value = health;

        if (_value <= 0f)
            DestroyWithoutInvoke();
    }

    public virtual void TakeDamage(float damage)
    {
        _value = Mathf.Clamp(_value - damage, 0f, _maxValue);
        if (_value <= 0f)
        {
            DestroyMe();
        }
        else if (damage > 0f)
        {
            _animator.SetTrigger("Hit");
        }
    }

    public virtual void TakeDamage(float damage, Vector3 attackerPosition)
    {
        TakeDamage(damage);
        if (damage <= 0f) return;

        Vector3 direction = (transform.position - attackerPosition).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction);
        Instantiate(_hurtParticles, _controller.CenterPosition, rotation);

        BattleStart();
    }

    public void BattleStart()
    {
        if (_inBattle) return;

        _inBattle = true;
    }

    public void BattleEnd()
    {
        if (!_inBattle) return;

        _inBattle = false;
    }

    protected virtual void InitComponents()
    {
        if (_controller == null)
            _controller = GetComponent<CreatureController>();
    }

    protected virtual void InitValues()
    {
        _value = _maxValue;
        _isAlive = true;
    }

    protected virtual void DestroyMe()
    {
        if (!IsAlive) return;
        OnDestroyed?.Invoke(this);
        DestroyWithoutInvoke();
    }

    private void DestroyWithoutInvoke()
    {
        _isAlive = false;
        _animator.SetBool("IsDeath", true);
        _animator.SetTrigger("Death");
        _controller.DisableLook();
        _controller.DisableMove();
        _controller.DisablePhysics();
        _controller.enabled = false;
        enabled = false;
    }
}
