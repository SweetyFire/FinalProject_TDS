using System;
using UnityEngine;

public abstract class AbilityArea : AbilityBase
{
    [Header("Area")]
    [SerializeField] protected float _radius;
    [SerializeField] protected int _maxTargets = 10;
    [SerializeField] protected LayerMask _creatureMask;
    [SerializeField] protected LayerMask _groundMask;

    protected Collider[] _overlapCreatures;

    protected virtual void Awake()
    {
        ResizeOverlapCreatureCollider();
    }

    protected void ResizeOverlapCreatureCollider()
    {
        _overlapCreatures = new Collider[_maxTargets];
    }

    protected void MakeActionWithOverlapCreatures(Action<CreatureController> action, bool noNeedToSee = false)
    {
        if (action == null) return;
        if (Physics.OverlapSphereNonAlloc(transform.position, _radius, _overlapCreatures, _creatureMask) <= 0) return;

        for (int i = _overlapCreatures.Length - 1; i >= 0; i--)
        {
            if (_overlapCreatures[i] == null) continue;
            if (!_overlapCreatures[i].TryGetComponent(out CreatureController controller)) continue;
            if (controller.Team == _owner.Controller.Team) continue;

            if (!noNeedToSee)
                if (Physics.Linecast(_owner.Controller.CenterPosition, controller.CenterPosition, _groundMask)) continue;

            action.Invoke(controller);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
#endif
}
