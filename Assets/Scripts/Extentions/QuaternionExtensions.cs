using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CanTemplate.Extensions
{
    public static class QuaternionExtensions
    {
        ///<summary>Clamps and returns euler angles of target quartenion between -180,180.</summary>
        ///<param name ="setEuler">Sets returned euler angles to target quartenion's euler angles.</param>
        public static Vector3 ClampEuler(this Quaternion rot, bool setEuler = false)
        {
            Vector3 rotEuler = rot.eulerAngles;
            rotEuler.y = rotEuler.y > 180 ? rotEuler.y - 360 : rotEuler.y;
            rotEuler.x = rotEuler.x > 180 ? rotEuler.x - 360 : rotEuler.x;
            rotEuler.z = rotEuler.z > 180 ? rotEuler.z - 360 : rotEuler.z;
            if (setEuler) rot.eulerAngles = rotEuler;
            return rotEuler;
        }
    }
}
