using UnityEngine;

public class AbilityDash : AbilityBase
{
    [Header("Dash")]
    [SerializeField] protected VectorDirection _direction = VectorDirection.Forward;
    [SerializeField] protected float _speed;
    [SerializeField] protected float _maxDistance;

    public override void Activate()
    {
        _owner.Controller.MoveDistance(_direction, _speed, _maxDistance).SetOnComplete(Ended);
    }

    public void Complete()
    {
        _owner.Controller.CompletePushByDistance();
    }

    protected void Ended()
    {
        OnCompleted?.Invoke();
    }

    protected Vector3 GetDirection()
    {
        return _owner.transform.GetDirection(_direction);
    }
}
