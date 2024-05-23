using UnityEngine;

public class AbilityJump : AbilityDash
{
    [Header("Jump")]
    [SerializeField] private float _jumpStrength;
    [SerializeField] private VectorDirection _jumpDirection = VectorDirection.Up;

    public override void Init(AbilityCaster caster)
    {
        base.Init(caster);
        enabled = false;
    }

    private void Update()
    {
        CheckGroundedUpdate();
    }

    public override void Activate()
    {
        enabled = false;
        Vector3 dashVelocity = _owner.Controller.CalculatePushDirection(_direction);
        dashVelocity *= _speed;

        Vector3 jumpVelocity = _owner.Controller.CalculatePushDirection(_jumpDirection);
        jumpVelocity *= _jumpStrength;

        _owner.Controller.MoveDistance(dashVelocity + jumpVelocity, _maxDistance, false).SetOnComplete(StartCheckGround);
    }

    private void CheckGroundedUpdate()
    {
        if (!_owner.Controller.IsGrounded) return;
        enabled = false;
        Ended();
    }

    private void StartCheckGround()
    {
        enabled = true;
    }
}
