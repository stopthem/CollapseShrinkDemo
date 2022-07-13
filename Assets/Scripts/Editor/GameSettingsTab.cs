using UnityEngine;
using UnityEditor;
using System.IO;

#if UNITY_EDITOR
public class GameSettingsTab : EditorWindow
{
    private int _levelVal
    {
        get => PlayerPrefs.GetInt("next_level");
        set => PlayerPrefs.SetInt("next_level", value);
    }

    private int MaxLevelVal
    {
        get => gameInfo.maxLevel;
        set
        {
            gameInfo.maxLevel = value;
            EditorUtility.SetDirty(gameInfo);
        }
    }

    private GameInfo gameInfo;

    [MenuItem("Tools/Game Settings")]
    private static void ShowWindow()
    {
        var window = GetWindow<GameSettingsTab>();
        window.titleContent = new GUIContent("Game Settings");
        window.Show();
    }

    private void OnGUI()
    {
        gameInfo = (GameInfo)EditorGUILayout.ObjectField("Game Info", Resources.Load<GameInfo>("Game Info"),
            typeof(GameInfo), true);

        EditorGUILayout.Space(5);

        if (gameInfo != null)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(new GUIContent("Level", "The current level number."),
                GUILayout.Width(50));
            _levelVal = EditorGUILayout.IntField(_levelVal, GUILayout.Width(50));

            GUILayout.Space(20);

            EditorGUILayout.LabelField(new GUIContent("Max Level", "The max level number."), GUILayout.Width(62.5f));
            MaxLevelVal = EditorGUILayout.IntField(gameInfo.maxLevel, GUILayout.Width(50));

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Set current level", GUILayout.Width(120), GUILayout.Height(30)))
            {
                PlayerPrefs.SetInt("next_level", _levelVal);
                int levelMod = _levelVal % MaxLevelVal;
                if (levelMod == 0) levelMod = MaxLevelVal;
                PlayerPrefs.SetInt("next_levelSc", levelMod);
            }

            if (GUILayout.Button("Update with \n Resources/Levels", GUILayout.Width(125), GUILayout.Height(30)))
                MaxLevelVal = Resources.LoadAll<LevelInfo>("Levels/").Length;


            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(15);

            EditorGUILayout.LabelField("PlayerPrefs", EditorStyles.boldLabel);
            if (GUILayout.Button("Delete PlayerPrefs", GUILayout.Width(120), GUILayout.Height(30)))
            {
                PlayerPrefs.DeleteAll();
                _levelVal = 1;
            }
        }
    }
}
#endif