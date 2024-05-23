using System.Globalization;
using UnityEngine;

public static class QuaternionExtensions
{
    public static bool TryParse(string value, out Quaternion result)
    {
        result = Quaternion.identity;

        string[] splitted = value[1..^1].Split(',');
        if (!float.TryParse(splitted[0], NumberStyles.Any, CultureInfo.InvariantCulture, out float x)) return false;
        if (!float.TryParse(splitted[1], NumberStyles.Any, CultureInfo.InvariantCulture, out float y)) return false;
        if (!float.TryParse(splitted[2], NumberStyles.Any, CultureInfo.InvariantCulture, out float z)) return false;
        if (!float.TryParse(splitted[3], NumberStyles.Any, CultureInfo.InvariantCulture, out float w)) return false;

        result = new Quaternion(x, y, z, w);
        return true;
    }
}
