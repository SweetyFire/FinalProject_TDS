using UnityEngine;

public class AbilityRepulsion : AbilityArea
{
    [Header("Repulsion")]
    [SerializeField] private float _force;

    protected override void ActivateAbility()
    {
        MakeActionWithOverlapCreatures(Repulsion);
    }

    private void Repulsion(CreatureController controller)
    {
        Vector3 pushDirection = (controller.transform.position - transform.position).normalized;
        controller.Push(pushDirection * _force);
    }
}
