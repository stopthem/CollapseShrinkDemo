using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
namespace CanTemplate.Editor
{
    public class GameSettingsTab : EditorWindow
    {
        private int LevelVal
        {
            get => PlayerPrefs.GetInt("next_level");
            set => PlayerPrefs.SetInt("next_level", value);
        }

        private int MaxLevelVal
        {
            get => _gameInfo.maxLevel;
            set
            {
                _gameInfo.maxLevel = value;
                EditorUtility.SetDirty(_gameInfo);
            }
        }

        private int MoneyAmount
        {
            get => PlayerPrefs.GetInt("money_amount", 0);
            set => PlayerPrefs.SetInt("money_amount", value);
        }

        private GameInfo _gameInfo;

        [MenuItem("Tools/Game Settings")]
        private static void ShowWindow()
        {
            var window = GetWindow<GameSettingsTab>();
            window.titleContent = new GUIContent("Game Settings");
            window.Show();
        }

        private void OnGUI()
        {
            _gameInfo = (GameInfo)EditorGUILayout.ObjectField("Game Info", Resources.Load<GameInfo>("Game Info"),
                typeof(GameInfo), true);

            EditorGUILayout.Space(5);

            if (_gameInfo is null) return;

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(new GUIContent("Level", "The current level number."),
                GUILayout.Width(50));
            LevelVal = EditorGUILayout.IntField(LevelVal, GUILayout.Width(50));

            GUILayout.Space(20);

            EditorGUILayout.LabelField(new GUIContent("Max Level", "The max level number."), GUILayout.Width(62.5f));
            MaxLevelVal = EditorGUILayout.IntField(_gameInfo.maxLevel, GUILayout.Width(50));

            GUILayout.Space(20);

            EditorGUILayout.LabelField(new GUIContent("Money", "The money amount."), GUILayout.Width(62.5f));
            MoneyAmount = EditorGUILayout.IntField(MoneyAmount, GUILayout.Width(50));

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Set current level", GUILayout.Width(120), GUILayout.Height(30)))
            {
                PlayerPrefs.SetInt("next_level", LevelVal);
                var levelMod = LevelVal % MaxLevelVal;
                if (levelMod == 0) levelMod = MaxLevelVal;
                PlayerPrefs.SetInt("next_levelSc", levelMod);
            }

            if (GUILayout.Button("Update with \n Resources/Levels", GUILayout.Width(125), GUILayout.Height(30)))
                MaxLevelVal = Resources.LoadAll<LevelInfo>("Levels/").Length;


            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(15);

            EditorGUILayout.LabelField("PlayerPrefs", EditorStyles.boldLabel);
            if (!GUILayout.Button("Delete PlayerPrefs", GUILayout.Width(120), GUILayout.Height(30))) return;

            PlayerPrefs.DeleteAll();
            LevelVal = 1;
        }
    }
}
#endif