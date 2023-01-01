using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ComponentExtensions
{
    public static T GetComponentInChildsOrParents<T>(this Component component, bool includeInactive = false) => component.GetComponentInParent<T>(includeInactive) ?? component.GetComponentInChildren<T>(includeInactive);

    public static bool TryGetComponentInParent<T>(this Component component, out T foundComponent, bool includeInactive = false)
    {
        foundComponent = component.GetComponentInParent<T>(includeInactive);
        return foundComponent != null;
    }

    public static bool TryGetComponentInChilds<T>(this Component component, out T foundComponent, bool includeInactive = false)
    {
        foundComponent = component.GetComponentInChildren<T>(includeInactive);
        return foundComponent != null;
    }

    public static void TryGetOrAddComponent<T>(this Component component, out T foundComponent) where T : Component
    {
        if (!component.TryGetComponent(out foundComponent))
        {
            foundComponent = component.gameObject.AddComponent<T>();
        }
    }
}