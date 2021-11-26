using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CanTemplate.Extensions
{
    public static class TransformExtensions
    {
        public static void CopyTransform(this Transform t, Transform toTransform, bool local = false, bool useToScale = true, bool setParent = true)
        {
            if (setParent) t.SetParent(toTransform.parent);

            if (local)
            {
                t.localPosition = toTransform.localPosition;
                t.localRotation = toTransform.localRotation;
            }
            else
            {
                t.rotation = toTransform.rotation;
                t.position = toTransform.position;
            }

            if (useToScale) t.localScale = toTransform.localScale;
        }
    }
}
