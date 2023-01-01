using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CanTemplate.Extensions
{
    public static class VectorExtensions
    {
        public static Vector2 XY(this Vector3 v) => new(v.x, v.y);

        ///<summary>Changes given vectors x and returns a new vector.</summary>
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
        public static Vector2 WithY(this Vector2 v, float y, bool relative = false) => new Vector2(v.x, relative ? v.y + y : y);

        public static float GetRandom(this Vector2 v) => Random.Range(v.x, v.y);
        public static int GetRandomInt(this Vector2 v) => (int)Random.Range(v.x, v.y);

        public static int GetRandom(this Vector2Int v) => Random.Range(v.x, v.y);

        ///<summary>Returns a direction from target vector to given vector.</summary>
        ///<param name ="normalized">Returns calculated vector with a magnitude of 1.</param>
        public static Vector3 GetDir(this Vector3 from, Vector3 to, bool normalized = true)
        {
            var dir = to - from;
            return normalized ? dir.normalized : dir;
        }

        /// <summary>
        /// InverseLerps given float between target vector2.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="value">give a value between range.x and range.y</param>
        /// <returns></returns>
        public static float InverseLerpBetween(this Vector2 range, float value) => Mathf.InverseLerp(range.x, range.y, value);

        /// <summary>
        /// Lerps given float between target vector2.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="t">give a normalized time 0-1</param>
        /// <returns></returns>
        public static float LerpBetween(this Vector2 range, float t) => Mathf.Lerp(range.x, range.y, t);

        public static float ClampBetween(this Vector2 range, float value) => Mathf.Clamp(value, range.x, range.y);

        public static Vector2 Reversed(this Vector2 range) => new(range.y, range.x);

        public static Vector3 ToV3(this Vector2 v, float z = 0) => new(v.x, v.y, z);
    }
}