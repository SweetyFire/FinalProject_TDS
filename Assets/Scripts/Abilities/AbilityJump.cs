using UnityEngine;

public class AbilityJump : AbilityDash
{
    [Header("Jump")]
    [SerializeField] private float _jumpStrength;

    private float _groundCheckTimer;
    private bool _upMovement;

    protected override void ActivateAbility()
    {
        if (_activated) return;

        _groundCheckTimer = 0.5f;
        _upMovement = true;
        base.ActivateAbility();
    }

    protected override void Update()
    {
        base.Update();
        DisableWhenGroundedUpdate();
    }

    protected override void MoveOwnerFixedUpdate()
    {
        if (_upMovement)
        {
            Vector3 velocity = Vector3.up * _jumpStrength;
            velocity += GetDirection() * _speed;
            _owner.Controller.Move(velocity);
        }
        else
        {
            base.MoveOwnerFixedUpdate();
        }
    }

    protected override void AddCurrentDistanceFixedUpdate()
    {
        _currentDistance += _jumpStrength * Time.fixedDeltaTime;
    }

    protected override void DisableOnMaxDistance()
    {
        if (_currentDistance < _maxDistance) return;
        _upMovement = false;
    }

    private void DisableWhenGroundedUpdate()
    {
        if (!_activated) return;

        if (_groundCheckTimer > 0)
        {
            _groundCheckTimer -= Time.deltaTime;
            return;
        }

        if (_owner.Controller.IsGrounded)
        {
            Disable();
        }
    }
}
