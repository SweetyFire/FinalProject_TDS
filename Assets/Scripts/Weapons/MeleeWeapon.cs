using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    [Header("Melee")]
    [SerializeField] private SphereCollider _attackCollider;

    private List<CreatureHealth> _targets = new();

    protected override void AttackStart()
    {
        if (_targets.Count > 0)
        {
            _audioSource.Stop();
            _audioSource.clip = _impactSounds.GetRandom();
            _audioSource.SetRandomPitchAndVolume(0.9f, 1.1f, 0.6f, 0.7f);
            _audioSource.Play();
        }

        for (int i = _targets.Count - 1; i >= 0; i--)
        {
            if (_targets[i] == null || !_targets[i].IsAlive)
            {
                _targets.RemoveAt(i);
                continue;
            }

            _targets[i].TakeDamage(_damage, transform.position);
        }
    }

    public override void Pickup(CreatureWeapon creature)
    {
        base.Pickup(creature);

        Vector3 newPos = new(-_owner.AttachPoint.localPosition.x, _attackCollider.transform.localPosition.y, _attackCollider.transform.localPosition.z);
        _attackCollider.transform.localPosition = newPos;

        _attackCollider.enabled = true;
    }

    public override void Drop()
    {
        base.Drop();
        _attackCollider.enabled = false;
    }

    public void OnCreatureEnter(Collider collider)
    {
        if (_owner == null) return;
        if (!collider.TryGetComponent(out CreatureHealth health)) return;
        if (!health.IsAlive) return;
        if (health == _owner.Health) return;
        if (health.Team == _owner.Team) return;

        _targets.Add(health);
    }

    public void OnCreatureExit(Collider collider)
    {
        if (_owner == null) return;
        if (!collider.TryGetComponent(out CreatureHealth health)) return;
        if (!_targets.Contains(health)) return;

        _targets.Remove(health);
    }
}
