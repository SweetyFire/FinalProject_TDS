using UnityEngine;

public class EnemyHealth : CreatureHealth
{
    public new EnemyController Controller => _enemyController;
    protected EnemyController _enemyController;

    protected override void InitComponents()
    {
        base.InitComponents();
        if (_enemyController == null)
            _enemyController = GetComponent<EnemyController>();
    }

    public override void TakeDamage(float damage, Vector3 attackerPosition)
    {
        base.TakeDamage(damage, attackerPosition);
        Controller.Sight.RotateTo(attackerPosition);
    }

    protected override void DestroyMe()
    {
        base.DestroyMe();
        _enemyController.enabled = false;
        _enemyController.Sight.enabled = false;
    }
}
