using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CanTemplate.Extensions
{
    public static class ColorExtensions
    {
        ///<summary>Returns target color with given <see langword="a"/>.</summary>
        public static Color WithA(this Color color, float a)
        {
            var toColor = color;
            toColor.a = a;
            return toColor;
        }
    }
}
