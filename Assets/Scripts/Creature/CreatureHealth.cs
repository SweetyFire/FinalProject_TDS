using UnityEngine;

public abstract class CreatureHealth : MonoBehaviour
{
    [Header("Creature")]
    [SerializeField] protected float _maxValue = 100f;
    [SerializeField] protected int _team;
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
