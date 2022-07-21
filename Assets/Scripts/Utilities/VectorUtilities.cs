using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace CanTemplate.Utilities
{
    public static class VectorUtilities
    {
        /// <summary>
        /// Returns basically Vector3.Angle but can be negative.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        public static float CalculateAngleWithCross(Vector3 lhs, Vector3 rhs, CrossAxis crossAxis = CrossAxis.X)
        {
            var angle = Vector3.Angle(lhs, rhs);
            var cross = Vector3.Cross(lhs, rhs);

            if (CalculateNegativeCross(crossAxis is CrossAxis.X ? cross.x : crossAxis is CrossAxis.Y ? cross.y : cross.z)) angle = -angle;
            return angle;

            bool CalculateNegativeCross(float angleAxis, float comparingValue = 0) => angleAxis < comparingValue;
        }

        public enum CrossAxis
        {
            X,
            Y,
            Z
        }

        public static float InverseLerp(Vector3 a, Vector3 b, Vector3 value)
        {
            var ab = b - a;
            var av = value - a;
            return Vector3.Dot(av, ab) / Vector3.Dot(ab, ab);
        }
    }
}