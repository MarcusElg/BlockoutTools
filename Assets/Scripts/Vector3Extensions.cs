using UnityEngine;

namespace BlockoutTools
{
    public static class Vector3Extensions
    {
        public static Vector3 ToXZ(Vector3 input)
        {
            return new Vector3(input.x, 0, input.z);
        }

        public static Vector2 To2DXZ(Vector3 input)
        {
            return new Vector2(input.x, input.z);
        }

        public static float XZDistance(Vector3 one, Vector3 two)
        {
            return Vector3.Distance(ToXZ(one), ToXZ(two));
        }

        // Ignores Y-value
        public static Vector3 LeftFromForward(Vector3 forward)
        {
            forward = ToXZ(forward);
            return new Vector3(-forward.z, 0, forward.x).normalized;
        }

        public static Vector3 GetInwardsFromTangents(Vector3 inTangent, Vector3 outTangent)
        {
            return LeftFromForward(outTangent - inTangent);
        }
    }
}