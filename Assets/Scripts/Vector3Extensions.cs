using UnityEngine;

public static class Vector3Extensions
{
    public static Vector3 ToXZ (Vector3 input)
    {
        return new Vector3(input.x, 0, input.z);
    }

    public static float XZDistance (Vector3 one, Vector3 two)
    {
        return Vector3.Distance(ToXZ(one), ToXZ(two));
    }
}
