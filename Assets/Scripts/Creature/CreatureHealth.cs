using UnityEngine;

public abstract class CreatureHealth : MonoBehaviour
{
    [Header("Creature")]
    [SerializeField] protected float _maxValue = 100f;
    [SerializeField] protected int _team;
    [Header("Particles")]
    [SerializeField] protected ParticleSystem _hurtParticles;
    protected float _value;

    public float MaxValue => _maxValue;
    public float Value => _value;

    public bool IsAlive => _value > 0;
    public int Team => _team;

    public CreatureController Controller => _controller;
    protected CreatureController _controller;

    protected virtual void Awake()
    {
        InitComponents();
        InitValues();
    }

    public virtual void TakeDamage(float damage)
    {
        _value = Mathf.Clamp(_value - damage, 0, _maxValue);
        if (_value <= 0)
        {
            DestroyMe();
        }
    }

    public void TakeDamage(float damage, Vector3 attackerPosition)
    {
        TakeDamage(damage);
        if (damage <= 0f) return;

        Vector3 direction = (transform.position - attackerPosition).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction);
        Instantiate(_hurtParticles, _controller.Center, rotation);
    }

    protected virtual void DestroyMe()
    {
        Destroy(gameObject);
    }

    protected virtual void InitValues()
    {
        _value = _maxValue;
    }

    protected virtual void InitComponents()
    {
        _controller = GetComponent<CreatureController>();
    }
}
