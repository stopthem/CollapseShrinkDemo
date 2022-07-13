using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CanTemplate.Utils
{
    public static class RigidbodyUtilities
    {
        public static Vector3 GetRBSCenterOfMass(Rigidbody[] bodies)
        {
            Vector3 centerOfMass = Vector3.zero;
            float totalMass = 0f;

            foreach (Rigidbody body in bodies)
            {
                centerOfMass += body.worldCenterOfMass * body.mass;
                totalMass += body.mass;
            }

            return centerOfMass / totalMass;
        }
    }
}