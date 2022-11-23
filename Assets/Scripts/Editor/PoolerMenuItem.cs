using UnityEditor;
using UnityEngine;

namespace CanTemplate.Pooling
{
    public class PoolerMenuItem : MonoBehaviour
    {
#if UNITY_EDITOR

        [MenuItem("GameObject/Pooling/Pooler", false, 10)]
        static void CreateCustomGameObject(MenuCommand menuCommand)
        {
            var go = new GameObject("Pooler");
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            go.AddComponent<Pooler>();
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }
#endif
    }
}
