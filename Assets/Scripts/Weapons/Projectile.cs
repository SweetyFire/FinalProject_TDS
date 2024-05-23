using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _maxTimeToDestroy = 20f;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private List<AudioClip> _hitSounds = new();

    private float _damage = 1f;
    private float _flySpeed = 10f;
    private float _maxFlyDistance = 5f;
    private int _team;
    private float _currentFlyDistance;
    private Rigidbody _rb;
    private Collider _collider;

    private float _destroyTimer = -1f;

    private void Awake()
    {
        InitComponents();
    }

    private void Update()
    {
        DestroyUpdate();
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
        _collider = GetComponent<Collider>();
    }

    private void MoveFixedUpdate()
    {
        if (_flySpeed == 0f) return;

        _rb.MovePosition(_rb.position + _flySpeed * transform.forward);
        _currentFlyDistance += _flySpeed;

        if (_currentFlyDistance > _maxFlyDistance)
        {
            DestroyMe();
        }
    }

    private void DestroyUpdate()
    {
        if (_destroyTimer == -1f) return;
        if (_destroyTimer > 0f)
        {
            _destroyTimer -= Time.deltaTime;
            return;
        }

        _destroyTimer = -1f;
        DestroyMe();
    }

    private void DealDamage(Collider other)
    {
        if (other.TryGetComponent(out CreatureHealth health))
        {
            if (health.Team != _team)
            {
                PlaySound();
                health.TakeDamage(_damage, transform.position);
            }
            else return;
        }

        transform.SetParent(other.transform);
        DisableFlight();
    }

    private void PlaySound()
    {
        _audioSource.Stop();
        _audioSource.clip = _hitSounds.GetRandom();
        _audioSource.SetRandomPitchAndVolume(0.9f, 1.1f, 0.6f, 0.7f);
        _audioSource.Play();
    }

    private void DisableFlight()
    {
        _collider.enabled = false;
        _flySpeed = 0f;
        _destroyTimer = _maxTimeToDestroy;
    }

    private void DestroyMe()
    {
        Destroy(gameObject);
    }
}
