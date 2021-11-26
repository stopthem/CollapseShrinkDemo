using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CanTemplate.Extensions
{
    public static class ColorExtensions
    {
        ///<summary>Returns target color with given a.</summary>
        ///<param name ="relative">If true, sets returned color to target color.</param>
        public static Color WithA(this Color color, float a, bool relative = false)
        {
            var toColor = color;
            toColor.a = a;
            if (relative) color = toColor;
            return toColor;
        }
    }
}
