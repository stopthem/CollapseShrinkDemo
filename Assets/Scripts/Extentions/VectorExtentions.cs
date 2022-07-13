using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CanTemplate.Extensions
{
    public static class VectorExtensions
    {
        public static Vector2 xy(this Vector3 v) => new Vector2(v.x, v.y);

        ///<summary>Changes given vectors x and returnes a new vector.</summary>
        ///<param name = "relative">If true, adds given value to target vectors x.</param>
        public static Vector3 WithX(this Vector3 v, float x, bool relative = false)
        {
            if (relative) x = v.x + x;
            var newVector = new Vector3(x, v.y, v.z);
            return newVector;
        }

        ///<summary>Changes given vectors y and returns a new vector.</summary>
        ///<param name = "relative">If true, adds given value to target vectors y.</param>
        public static Vector3 WithY(this Vector3 v, float y, bool relative = false)
        {
            if (relative) y = v.y + y;
            var newVector = new Vector3(v.x, y, v.z);
            return newVector;
        }

        ///<summary>Changes given vectors z and returns a new vector.</summary>
        ///<param name = "relative">If true, adds given value to target vectors z.</param>
        public static Vector3 WithZ(this Vector3 v, float z, bool relative = false)
        {
            if (relative) z = v.z + z;
            var newVector = new Vector3(v.x, v.y, z);
            return newVector;
        }

        public static Vector2 WithX(this Vector2 v, float x) => new Vector2(x, v.y);
        public static Vector2 WithY(this Vector2 v, float y) => new Vector2(v.x, y);

        public static float GetRandomBetweenXY(this Vector2 v) => Random.Range(v.x, v.y);

        ///<summary>Returns a direction from target vector to given vector.</summary>
        ///<param name ="normalized">Returns calculated vector with a magnitude of 1.</param>
        public static Vector3 GetDirection(this Vector3 from, Vector3 to, bool normalized = true)
        {
            Vector3 dir = to - from;
            return normalized ? dir.normalized : dir;
        }
    }
}