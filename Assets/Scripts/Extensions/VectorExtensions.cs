using System.Globalization;
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

    public static bool TryParse(string value, out Vector3 result)
    {
        result = Vector3.zero;

        string[] splitted = value[1..^1].Split(',');
        if (!float.TryParse(splitted[0], NumberStyles.Any, CultureInfo.InvariantCulture, out float x)) return false;
        if (!float.TryParse(splitted[1], NumberStyles.Any, CultureInfo.InvariantCulture, out float y)) return false;
        if (!float.TryParse(splitted[2], NumberStyles.Any, CultureInfo.InvariantCulture, out float z)) return false;

        result = new Vector3(x, y, z);
        return true;
    }
}
