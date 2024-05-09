using UnityEngine;

public class AbilityAreaDamage : AbilityArea
{
    [Header("Area Damage")]
    [SerializeField] private float _damage;
    [SerializeField] private ParticleSystem _particleSystem;

    private float _changeParentTimer;

    protected override void Update()
    {
        base.Update();
        ParticleChangeParentUpdate();
    }

    protected override void ActivateAbility()
    {
        _particleSystem.transform.ClearParent();
        _particleSystem.Play();
        _changeParentTimer = _particleSystem.main.duration;
        MakeActionWithOverlapCreatures(DealDamage);
        OnCompleted?.Invoke(this);
    }

    private void DealDamage(CreatureController controller)
    {
        controller.Health.TakeDamage(_damage);
    }

    private void ChangeParentToMe()
    {
        _particleSystem.transform.SetParent(transform);
        _particleSystem.transform.localPosition = Vector3.zero;
        _particleSystem.transform.localRotation = Quaternion.identity;
    }

    private void ParticleChangeParentUpdate()
    {
        if (_changeParentTimer <= -1f) return;

        if (_changeParentTimer > 0f)
        {
            _changeParentTimer -= Time.deltaTime;
            return;
        }

        ChangeParentToMe();
        _changeParentTimer = -1f;
    }
}
