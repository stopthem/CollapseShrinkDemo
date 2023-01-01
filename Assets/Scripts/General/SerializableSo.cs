using System.IO;
using UnityEditor;
using UnityEngine;

namespace CanTemplate.SaveSystem
{
    public abstract class SerializableSo : ScriptableObject
    {
        private string JsonPath => Path.Combine(Application.persistentDataPath, $"{name}.json");

        protected abstract void ResetVariables();

        public void Clear()
        {
            ResetVariables();
            DampToJson();
        }

        public void DampToJson()
        {
            var myContents = JsonUtility.ToJson(this);
            File.WriteAllText(JsonPath, myContents);
        }

        public void DampFromJson()
        {
            if (!File.Exists(JsonPath) || string.IsNullOrEmpty(File.ReadAllText(JsonPath)))
                File.WriteAllText(JsonPath, JsonUtility.ToJson(this));

            var jsonTexts = File.ReadAllText(JsonPath);
            JsonUtility.FromJsonOverwrite(jsonTexts, this);
        }
    }

    /// <summary>
    /// This is for OnFirstTime Scriptable Event so SerializableSo user can reset its SerializableSo. Its not necessary but for order.
    /// </summary>
    interface ISerializableSoUser
    {
        void FirstTime();
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(SerializableSo), true), CanEditMultipleObjects]
    public class SerializableSoEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var serializableSo = target as SerializableSo;

            GUILayout.Space(20);

            if (GUILayout.Button("Damp From Json (Values you see might be outdated)"))
            {
                serializableSo.DampFromJson();
            }

            EditorGUILayout.HelpBox("Any change you do in the inspector is automatically damped to json", MessageType.Info);
            if (GUILayout.Button("Damp To Json"))
            {
                serializableSo.DampToJson();
            }

            if (GUILayout.Button("Clear Json"))
            {
                serializableSo.Clear();
            }

            if (GUI.changed)
            {
                serializableSo.DampToJson();
            }
        }
    }
#endif
}