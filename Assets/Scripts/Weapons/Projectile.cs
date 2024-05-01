using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float _damage = 1f;
    private float _flySpeed = 10f;
    private float _maxFlyDistance = 5f;
    private Rigidbody _rb;
    private int _team;
    private float _currentFlyDistance;

    private void Awake()
    {
        InitComponents();
    }

    private void FixedUpdate()
    {
        MoveFixedUpdate();
    }

    private void OnTriggerEnter(Collider other)
    {
        DealDamage(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        DealDamage(collision.collider);
    }

    public void Init(float flySpeed, float maxFlyDistance, Weapon weapon)
    {
        _flySpeed = flySpeed;
        _maxFlyDistance = maxFlyDistance;
        _damage = weapon.Damage;
        _team = weapon.Owner.Team;
    }

    private void InitComponents()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void MoveFixedUpdate()
    {
        _rb.MovePosition(_rb.position + _flySpeed * transform.forward);
        _currentFlyDistance += _flySpeed;

        if (_currentFlyDistance > _maxFlyDistance)
        {
            DestroyMe();
        }
    }

    private void DealDamage(Collider other)
    {
        if (other.TryGetComponent(out CreatureHealth health))
        {
            if (health.Team != _team)
            {
                health.TakeDamage(_damage);
            }
        }

        DestroyMe();
    }

    private void DestroyMe()
    {
        Destroy(gameObject);
    }
}
