using System;

public class PlayerHealth : CreatureHealth
{
    public event Action<PlayerHealth> OnHealthUpdated;
    public event Action<PlayerHealth> OnMaxHealthUpdated;

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        OnHealthUpdated?.Invoke(this);
    }

    protected override void InitValues()
    {
        base.InitValues();
        OnMaxHealthUpdated?.Invoke(this);
        OnHealthUpdated?.Invoke(this);
    }

    protected override void DestroyMe()
    {

    }
}
