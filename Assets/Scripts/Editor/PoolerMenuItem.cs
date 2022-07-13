using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PoolerMenuItem : MonoBehaviour
{
#if UNITY_EDITOR

    [MenuItem("GameObject/CanTemplate/Pooler", false, 10)]
    static void CreateCustomGameObject(MenuCommand menuCommand)
    {
        GameObject go = new GameObject("Pooler");
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        go.AddComponent<Pooler>();
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
    }
#endif
}
