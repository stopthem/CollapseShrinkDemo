using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CanTemplate.Utilities
{
    public static class NavMeshUtilities
    {
        public static Vector3 GetRandomPoint(Vector3 center, float maxDistance)
        {
            var randomPos = Random.insideUnitSphere * maxDistance + center;

            NavMesh.SamplePosition(randomPos, out var hit, maxDistance, NavMesh.AllAreas);

            return hit.position;
        }
    }
}