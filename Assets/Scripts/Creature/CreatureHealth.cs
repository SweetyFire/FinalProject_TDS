using UnityEngine;

public abstract class CreatureHealth : MonoBehaviour
{
    [Header("Creature")]
    [SerializeField] protected float _maxValue = 100f;
    protected float _value;

    public bool IsAlive => _value > 0;

    public int Team => _team;
    protected int _team;

    private void Awake()
    {
        InitValues();
    }

    public void TakeDamage(float damage)
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

    private void InitValues()
    {
        _value = _maxValue;
    }
}
