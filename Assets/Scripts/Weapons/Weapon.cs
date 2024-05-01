using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [Header("Weapon")]
    [SerializeField] protected float _damage;
    [SerializeField] protected float _attackRate;
    [SerializeField] protected float _attackDistance;

    public float Damage => _damage;
    public float AttackRate => _attackRate;
    public float AttackDistance => _attackDistance;
    public CreatureWeapon Owner => _owner;

    protected CreatureWeapon _owner;
    protected Collider _collider;
    protected float _timeToAttack;

    private bool _initialized;

    protected virtual void Awake()
    {
        InitComponents();
    }

    protected virtual void Update()
    {
        TimeToAttackUpdate();
    }

    private void OnTriggerEnter(Collider other)
    {
        TryPickup(other);
    }

    public virtual void Pickup(CreatureWeapon creature)
    {
        InitComponents();
        _owner = creature;
        _collider.enabled = false;
    }

    public virtual void Drop()
    {
        _owner = null;
        _collider.enabled = true;
    }

    public void Attack()
    {
        if (_timeToAttack > 0) return;
        if (_owner == null) return;

        _timeToAttack = _attackRate;
        AttackStart();
    }

    protected abstract void AttackStart();

    private void InitComponents()
    {
        if (_initialized) return;

        _collider = GetComponent<Collider>();
        _initialized = true;
    }

    private void TryPickup(Collider other)
    {
        if (!other.TryGetComponent(out CreatureWeapon weapon)) return;
        if (!weapon.TryAddWeapon(this)) return;
        Pickup(weapon);
    }

    private void TimeToAttackUpdate()
    {
        if (_timeToAttack > 0f)
        {
            _timeToAttack -= Time.deltaTime;
        }
    }
}
