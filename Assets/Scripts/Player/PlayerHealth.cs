using UnityEngine;

public class PlayerHealth : CreatureHealth
{
    [Header("UI")]
    [SerializeField] private BarUI _healthbar;

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        UpdateBar();
    }

    protected override void InitValues()
    {
        base.InitValues();
        UpdateBar();
    }

    protected override void DestroyMe()
    {

    }

    private void UpdateBar()
    {
        _healthbar.UpdateValue(Value, MaxValue);
    }
}
