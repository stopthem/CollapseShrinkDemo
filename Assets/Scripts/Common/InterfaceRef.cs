using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class InterfaceRef<T> : ISerializationCallbackReceiver where T : class
{
    [HideInInspector] public Object target;

    public T I => target as T;

    public static implicit operator bool(InterfaceRef<T> ir) => ir.target != null;

    private void OnValidate()
    {
        if (target is T) return;

        if (target is not GameObject go) return;

        target = null;

        foreach (Component c in go.GetComponents<Component>())
        {
            if (c is not T) continue;

            target = c;

            break;
        }
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize() => OnValidate();

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(InterfaceRef<>))]
public class InterfaceRefDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        var target = property.FindPropertyRelative("target");
        target.objectReferenceValue = EditorGUI.ObjectField(position, target.objectReferenceValue, typeof(InterfaceRef<Object>), true);

        EditorGUI.EndProperty();
    }
}
#endif