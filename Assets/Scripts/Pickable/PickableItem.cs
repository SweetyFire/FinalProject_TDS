using UnityEngine;
using UnityEngine.Events;

public class PickableItem : MonoBehaviour
{
    [SerializeField] private Transform _destroyGameObject;
    public UnityEvent<CreatureHealth> OnPickup;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out CreatureHealth creature)) return;
        Pickup(creature);
    }

    public void Pickup(CreatureHealth creature)
    {
        OnPickup?.Invoke(creature);
        if (_destroyGameObject != null)
            Destroy(_destroyGameObject.gameObject);
    }
}
