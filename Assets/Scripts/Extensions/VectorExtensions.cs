using UnityEngine;

public static class VectorExtensions
{
    public static Vector3 RotateTo(this Vector3 vector, float x, float y, float z)
    {
        return Quaternion.Euler(x, y, z) * vector;
    }

    public static Vector3 RandomPointAroundXZ(this Vector3 vector, float radius)
    {
        Vector3 randomPos = Random.insideUnitSphere * radius + vector;
        randomPos.y = vector.y;
        return randomPos;
    }
}
