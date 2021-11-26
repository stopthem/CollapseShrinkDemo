using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class SetLevelTab : EditorWindow
{
    private int _levelVal = 1;
    private int _maxLevelVal
    {
        get => PlayerPrefs.GetInt("max_level", 1);
        set => PlayerPrefs.SetInt("max_level", value);
    }


    [MenuItem("Tools/Game Settings")]
    private static void ShowWindow()
    {
        var window = GetWindow<SetLevelTab>();
        window.titleContent = new GUIContent("Game Settings");
        window.Show();
    }

    private void OnEnable()
    {
        _levelVal = PlayerPrefs.GetInt("next_level", 1);
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(new GUIContent("Level", "The current level number."),
            GUILayout.Width(50));
        _levelVal = EditorGUILayout.IntField(_levelVal, GUILayout.Width(50));

        GUILayout.Space(20);

        EditorGUILayout.LabelField(new GUIContent("Max Level", "The max level number."), GUILayout.Width(62.5f));
        _maxLevelVal = EditorGUILayout.IntField(_maxLevelVal, GUILayout.Width(50));

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Set current level", GUILayout.Width(120), GUILayout.Height(30)))
        {
            PlayerPrefs.SetInt("next_level", _levelVal);
            int levelMod = _levelVal % _maxLevelVal;
            if (levelMod == 0) levelMod = _maxLevelVal;
            PlayerPrefs.SetInt("next_levelSc", levelMod);
        }

        if (GUILayout.Button("Update with \n Resources/Levels", GUILayout.Width(125), GUILayout.Height(30)))
            _maxLevelVal = Resources.LoadAll<LevelInfo>("Levels/").Length;

        GUILayout.EndHorizontal();

        GUILayout.Space(15);

        EditorGUILayout.LabelField("PlayerPrefs", EditorStyles.boldLabel);
        if (GUILayout.Button("Delete PlayerPrefs", GUILayout.Width(120), GUILayout.Height(30)))
            PlayerPrefs.DeleteAll();
    }
}
#endif