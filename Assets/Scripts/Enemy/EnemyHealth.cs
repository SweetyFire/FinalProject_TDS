
public class EnemyHealth : CreatureHealth
{
    public EnemyController Controller => _controller;
    private EnemyController _controller;

    protected override void Awake()
    {
        base.Awake();
        InitComponents();
    }

    private void InitComponents()
    {
        _controller = GetComponent<EnemyController>();
    }
}
