using UnityEngine;

public static class TransformExtensions
{
    public static Vector3 GetDirection(this Transform transform, VectorDirection direction)
    {
        return direction switch
        {
            VectorDirection.Forward => transform.forward,
            VectorDirection.Backward => -transform.forward,
            VectorDirection.Right => transform.right,
            VectorDirection.Left => -transform.right,
            VectorDirection.Up => transform.up,
            VectorDirection.Down => -transform.up,
            _ => Vector3.zero,
        };
    }

    public static void ClearParent(this Transform transform)
    {
        transform.SetParent(null);
    }
}
