using UnityEngine;

public class EnemySight : MonoBehaviour
{
    private EnemyController _owner;

    private void Awake()
    {
        _owner = GetComponentInParent<EnemyController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        _owner.OnEnemyEnterTrigger(other);
    }

    private void OnTriggerExit(Collider other)
    {
        _owner.OnEnemyExitTrigger(other);
    }
}
