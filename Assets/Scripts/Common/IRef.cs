using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class IRef<T> : ISerializationCallbackReceiver where T : class
{
    [HideInInspector] public Object target;

    public T I => target as T;

    public static implicit operator bool(IRef<T> ir) => ir.target != null;

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
[CustomPropertyDrawer(typeof(IRef<>))]
public class IRefDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        var target = property.FindPropertyRelative("target");
        target.objectReferenceValue = EditorGUI.ObjectField(position, target.objectReferenceValue, typeof(IRef<Object>), true);

        EditorGUI.EndProperty();
    }
}
#endif