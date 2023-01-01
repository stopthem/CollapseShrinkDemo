using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColliderExtensions
{
    public static Vector3 RandomPointInBounds(this Collider collider)
    {
        var bounds = collider.bounds;
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }
}