using UnityEngine;

public class AbilityDash : AbilityBase
{
    [Header("Dash")]
    [SerializeField] protected VectorDirection _direction;
    [SerializeField] protected float _speed;
    [SerializeField] protected float _maxDistance;

    protected float _currentDistance;
    protected bool _activated;

    private void FixedUpdate()
    {
        if (!_activated) return;

        MoveOwnerFixedUpdate();
        AddCurrentDistanceFixedUpdate();
        DisableOnMaxDistance();
    }

    public override void Activate()
    {
        if (_activated) return;

        _currentDistance = 0f;
        Enable();
    }

    protected virtual void MoveOwnerFixedUpdate()
    {
        if (_direction == VectorDirection.Movement)
        {
            _owner.Controller.MoveToMovementDirection(_speed);
        }
        else if (_direction == VectorDirection.Look)
        {
            _owner.Controller.MoveToLookDirection(_speed);
        }
        else
        {
            Vector3 velocity = GetDirection() * _speed;
            _owner.Controller.Move(velocity.x, velocity.z);
        }
    }

    protected virtual void AddCurrentDistanceFixedUpdate()
    {
        _currentDistance += _speed * Time.fixedDeltaTime;
    }

    protected virtual void DisableOnMaxDistance()
    {
        if (_currentDistance < _maxDistance) return;
        Disable();
    }

    protected Vector3 GetDirection()
    {
        return _owner.transform.GetDirection(_direction);
    }

    protected void Enable()
    {
        _activated = true;
    }

    protected void Disable()
    {
        if (!_activated) return;
        _activated = false;
        OnCompleted?.Invoke();
    }
}
