using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace CanTemplate.Extensions
{
    public static class FloatExtensions
    {
        public static bool IsWithinRange(this float f, float min, float max) => f >= min && f <= max;
        public static bool IsWithinRange(this float f, Vector2 range, bool maxIncluded = true) => f >= range.x && (maxIncluded ? f <= range.y : f < range.y);

        public static bool Approximately(this float f, float otherF, float range = .05f) => Mathf.Abs(f - otherF) <= range;
    }
}