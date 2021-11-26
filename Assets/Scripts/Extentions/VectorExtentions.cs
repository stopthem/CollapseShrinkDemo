using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CanTemplate.Extensions
{
    public static class VectorExtensions
    {
        public static Vector2 xy(this Vector3 v) => new Vector2(v.x, v.y);

        public static Vector3 WithX(this Vector3 v, float x) => new Vector3(x, v.y, v.z);
        public static Vector3 WithY(this Vector3 v, float y) => new Vector3(v.x, y, v.z);
        public static Vector3 WithZ(this Vector3 v, float z) => new Vector3(v.x, v.y, z);
        ///<summary>Adds given values to target vector3 and returnes it.</summary>
        ///<param name ="setIt">Sets the returned vector to target vector</param>
        public static Vector3 SetRelative(this Vector3 v, float x = 0, float y = 0, float z = 0, bool setIt = true)
        {
            Vector3 newVector = new Vector3(v.x + x, v.y + y, v.z + z);
            if (setIt) v = newVector;
            return newVector;
        }

        public static Vector2 WithX(this Vector2 v, float x) => new Vector2(x, v.y);
        public static Vector2 WithY(this Vector2 v, float y) => new Vector2(v.x, y);

        ///<param name ="normalized">Clamps directions every axis magnitude to 1.</param>
        public static Vector3 GetDirection(this Vector3 from, Vector3 to, bool normalized = true)
        {
            Vector3 dir = to - from;
            return normalized ? dir.normalized : dir;
        }
    }
}
