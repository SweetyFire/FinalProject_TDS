using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [Header("Weapon")]
    [SerializeField] protected float _damage;
    [SerializeField] protected float _attackRate;

    protected CreatureWeapon _owner;
    protected Collider _collider;
    protected float _timeToAttack;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    private void Update()
    {
        if (_timeToAttack > 0f)
        {
            _timeToAttack -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        TryPickup(other);
    }

    public void Pickup(CreatureWeapon creature)
    {
        _owner = creature;
        _collider.enabled = false;
    }

    public void Drop()
    {
        _owner = null;
        _collider.enabled = true;
    }

    public abstract void Attack();

    private void TryPickup(Collider other)
    {
        if (!other.TryGetComponent(out CreatureWeapon weapon)) return;
        if (!weapon.TryAddWeapon(this)) return;
        Pickup(weapon);
    }
}
