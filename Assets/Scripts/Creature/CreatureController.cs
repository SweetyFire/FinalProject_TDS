using UnityEngine;

public abstract class CreatureController : MonoBehaviour
{
    public float Height => _collider.height;

    protected CapsuleCollider _collider;

    protected virtual void InitComponents()
    {
        _collider = GetComponent<CapsuleCollider>();
    }
}
