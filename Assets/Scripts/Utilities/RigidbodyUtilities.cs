using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CanTemplate.Utilities
{
    public static class RigidbodyUtilities
    {
        public static Vector3 GetRbsCenterOfMass(Rigidbody[] bodies)
        {
            var centerOfMass = Vector3.zero;
            var totalMass = 0f;

            foreach (var body in bodies)
            {
                centerOfMass += body.worldCenterOfMass * body.mass;
                totalMass += body.mass;
            }

            return centerOfMass / totalMass;
        }
    }
}