using UnityEngine;

public class AbilityRepulsion : AbilityArea
{
    [Header("Repulsion")]
    [SerializeField] private float _force;
    [SerializeField] private float _repulsionTime = 1f;
    [SerializeField] private int _maxRepulsionRigidbodies = 50;

    private Collider[] _rigidbodies;

    protected override void Awake()
    {
        base.Awake();
        _rigidbodies = new Collider[_maxRepulsionRigidbodies];
    }

    public override void Activate()
    {
        MakeActionWithOverlapCreatures(Repulsion);
        RepulsionRigidbodies();
        OnCompleted?.Invoke();
    }

    private void Repulsion(CreatureController controller)
    {
        Vector3 pushDirection = (controller.transform.position - transform.position).normalized;
        controller.Move(pushDirection * _force, _repulsionTime);
    }

    private void RepulsionRigidbodies()
    {
        if (Physics.OverlapSphereNonAlloc(transform.position, _radius, _rigidbodies, _groundMask) <= 0) return;

        for (int i = _rigidbodies.Length - 1; i >= 0; i--)
        {
            if (_rigidbodies[i] == null) continue;
            if (_rigidbodies[i].attachedRigidbody == null) continue;
            if (_rigidbodies[i].TryGetComponent(out CreatureController _)) continue;

            _rigidbodies[i].attachedRigidbody.AddExplosionForce(_force * 100f, transform.position, _radius);
        }
    }
}
