using UnityEngine;

public class EnemyWeapon : CreatureWeapon
{
    [SerializeField] private float _minAttackSpeed = 2f;
    [SerializeField] private float _maxAttackSpeed = 3f;

    private float _attackTimer;
    private EnemyController _enemyController;

    protected override void Update()
    {
        base.Update();
        AttackTargetInSightUpdate();
    }

    public override bool TryAddWeapon(Weapon weapon)
    {
        return false;
    }

    protected override void InitComponents()
    {
        base.InitComponents();
        _enemyController = GetComponent<EnemyController>();
    }

    private void AttackTargetInSightUpdate()
    {
        if (_attackTimer > 0)
        {
            _attackTimer -= Time.deltaTime;
            return;
        }

        if (_enemyController.CurrentTarget == null) return;
        if (!_enemyController.SeeCurrentTarget) return;

        float targetDistance = Vector3.Distance(transform.position, _enemyController.CurrentTarget.transform.position);
        float attackDistance = _weapon.AttackDistance + 2f;
        if (targetDistance > attackDistance) return;

        Attack();
        _attackTimer = Random.Range(_minAttackSpeed, _maxAttackSpeed);
    }
}
