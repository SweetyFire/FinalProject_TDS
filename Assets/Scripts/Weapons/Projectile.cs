using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float _damage = 1f;
    private float _flySpeed = 10f;
    private float _maxFlyTime = 5f;
    private Rigidbody _rb;
    private int _team;

    private void Awake()
    {
        InitComponents();
    }

    private void Update()
    {
        FlyTimeUpdate();
    }

    private void FixedUpdate()
    {
        _rb.MovePosition(_rb.position + _flySpeed * Time.fixedDeltaTime * transform.forward);
    }

    private void OnTriggerEnter(Collider other)
    {
        DealDamage(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        DealDamage(collision.collider);
    }

    public void Init(float flySpeed, float maxFlyTime, Weapon weapon)
    {
        _flySpeed = flySpeed;
        _maxFlyTime = maxFlyTime;
        _damage = weapon.Damage;
        _team = weapon.Owner.Team;
    }

    private void InitComponents()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FlyTimeUpdate()
    {
        if (_maxFlyTime > 0f)
        {
            _maxFlyTime -= Time.deltaTime;
        }
        else
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
