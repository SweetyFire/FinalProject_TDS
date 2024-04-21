using UnityEngine;

public static class VectorExtensions
{
    public static Vector3 RotateTo(this Vector3 vector, float x, float y, float z)
    {
        return Quaternion.Euler(x, y, z) * vector;
    }
}
