using UnityEngine;

public class EnemyWeapon : CreatureWeapon
{
    [Header("Enemy")]
    [SerializeField] private float _minAttackSpeed = 2f;
    [SerializeField] private float _maxAttackSpeed = 3f;
    [SerializeField] private EnemySight _enemySight;

    private float _attackTimer;

    protected override void Update()
    {
        base.Update();
        AttackTargetInSightUpdate();
    }

    public override bool TryAddWeapon(Weapon weapon) => false;

    private void AttackTargetInSightUpdate()
    {
        if (DisabledAttack) return;

        if (_attackTimer > 0)
        {
            _attackTimer -= Time.deltaTime;
            return;
        }

        if (!_enemySight.SeeCurrentTarget) return;
        if (_enemySight.Target == null) return;

        float targetDistance = Vector3.Distance(transform.position, _enemySight.Target.transform.position);
        float attackDistance = _weapon.AttackDistance + 2f;
        if (targetDistance > attackDistance) return;

        Attack();
        _attackTimer = Random.Range(_minAttackSpeed, _maxAttackSpeed);
    }
}
