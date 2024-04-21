using UnityEngine;

public class Bullet : MonoBehaviour
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
            Destroy(gameObject);
        }
    }

    private void DealDamage(Collider other)
    {
        if (!other.TryGetComponent(out CreatureHealth health)) return;
        if (health.Team == _team) return;
        health.TakeDamage(_damage);
    }
}
