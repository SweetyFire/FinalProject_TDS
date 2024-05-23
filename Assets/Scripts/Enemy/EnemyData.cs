using UnityEngine;

public class EnemyData
{
    public Vector3 position;
    public Quaternion rotation;
    public float health;

    public void Clear()
    {
        position = default;
        rotation = default;
        health = default;
    }
}
