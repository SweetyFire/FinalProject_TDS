using UnityEngine;

public class SummonMeteor : Summonable
{
    [Header("Meteor")]
    [SerializeField] private float _lifetime = 1f;
    [Header("Abilities")]
    [SerializeField] private AbilityRepulsion _repulsion;
    [SerializeField] private AbilityAreaDamage _areaDamage;

    private float _timeToDestroy = -1f;

    private void Update()
    {
        DestroyTimerUpdate();
    }

    public void OnMeteorLanded()
    {
        _areaDamage.Activate();
        _repulsion.Activate();
        _timeToDestroy = _lifetime;
    }

    public override void Init(AbilityCaster caster)
    {
        base.Init(caster);
        _repulsion.Init(caster);
        _areaDamage.Init(caster);
    }

    private void DestroyTimerUpdate()
    {
        if (_timeToDestroy <= -1f) return;

        if (_timeToDestroy > 0f)
        {
            _timeToDestroy -= Time.deltaTime;
            return;
        }

        Destroy(gameObject);
    }
}
