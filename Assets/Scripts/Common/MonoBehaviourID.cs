using System;
using UnityEngine;
using System.Linq;

public class MonoBehaviourID : MonoBehaviour
{
    [SerializeField] private UniqueID id;

    public string ID => id.value;

    [ContextMenu("Force reset ID")]
    private void ResetId()
    {
        id.value = Guid.NewGuid().ToString();
        Debug.Log("Setting new ID on object: " + gameObject.name, gameObject);
    }

    //Need to check for duplicates when copying a gameobject/component
    public static bool IsUnique(string ıd)
    {
        return Resources.FindObjectsOfTypeAll<MonoBehaviourID>().Count(x => x.ID == ıd) == 1;
    }

    protected void OnValidate()
    {
        //If scene is not valid, the gameobject is most likely not instantiated (ex. prefabs)
        if (!gameObject.scene.IsValid())
        {
            id.value = string.Empty;
            return;
        }

        if (string.IsNullOrEmpty(ID) || !IsUnique(ID))
        {
            ResetId();
        }
    }

    [Serializable]
    private struct UniqueID
    {
        public string value;
    }

#if UNITY_EDITOR

    [UnityEditor.CustomPropertyDrawer(typeof(UniqueID))]
    private class UniqueIdDrawer : UnityEditor.PropertyDrawer
    {
        private const float ButtonWidth = 120;
        private const float Padding = 2;

        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            UnityEditor.EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = UnityEditor.EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            GUI.enabled = false;
            var valueRect = position;
            valueRect.width -= ButtonWidth + Padding;
            UnityEditor.SerializedProperty idProperty = property.FindPropertyRelative("Value");
            UnityEditor.EditorGUI.PropertyField(valueRect, idProperty, GUIContent.none);

            GUI.enabled = true;

            var buttonRect = position;
            buttonRect.x += position.width - ButtonWidth;
            buttonRect.width = ButtonWidth;
            if (GUI.Button(buttonRect, "Copy to clipboard"))
            {
                UnityEditor.EditorGUIUtility.systemCopyBuffer = idProperty.stringValue;
            }

            UnityEditor.EditorGUI.EndProperty();
        }
    }
#endif
}