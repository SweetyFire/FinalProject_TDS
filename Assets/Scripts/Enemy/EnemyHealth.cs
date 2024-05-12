
public class EnemyHealth : CreatureHealth
{
    public new EnemyController Controller => _enemyController;
    protected EnemyController _enemyController;

    protected override void InitComponents()
    {
        base.InitComponents();
        _enemyController = GetComponent<EnemyController>();
    }
}
