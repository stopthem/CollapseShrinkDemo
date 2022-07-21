using System.Collections;
using System.Collections.Generic;
using CanTemplate.Extensions;
using UnityEngine;

namespace CanTemplate.Utilities
{
    public static class QuaternionUtilities
    {
        public static Quaternion SmoothDamp(Quaternion current, Quaternion target, ref Vector3 currentVelocity, float smoothTime)
        {
            if (Time.timeScale == 0) return current;
            
            var c = current.eulerAngles;
            var t = target.eulerAngles;
            
            return Quaternion.Euler(
                Mathf.SmoothDampAngle(c.x, t.x, ref currentVelocity.x, smoothTime),
                Mathf.SmoothDampAngle(c.y, t.y, ref currentVelocity.y, smoothTime),
                Mathf.SmoothDampAngle(c.z, t.z, ref currentVelocity.z, smoothTime)
            );
        }
    }
}