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
        _owner.OnCreatureEnterTrigger(other);
    }

    private void OnTriggerExit(Collider other)
    {
        _owner.OnCreatureExitTrigger(other);
    }
}
