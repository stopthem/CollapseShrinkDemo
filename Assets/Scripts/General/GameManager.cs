using System;
using CanTemplate.Camera;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using CanTemplate.Extensions;
using CanTemplate.UI;
using ScriptableEvents.Events;
using UnityEditor;
using UnityEngine.Events;
using FileUtilities = CanTemplate.Utilities.FileUtilities;

namespace CanTemplate.GameManaging
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private float defaultDelayBeforeEnd = .5f;
        [SerializeField] private int targetFps = 60;

        public static GameStatus gameStatus = GameStatus.Menu;
        public UnityEvent onGameStarted, onGameSuccess, onGameFailed, onGameEnded;

        public static LevelInfo CurrentLevelInfo { get; private set; }
        public static int CurrentLevelCount => PlayerPrefs.GetInt("next_level", 1);
        public static int MaxLevel { get; private set; }

        private Tween _levelEndedTween;

        public SimpleScriptableEvent onGameFirstTime;

        private void Awake()
        {
            Instance = this;

            MaxLevel = Resources.Load<GameInfo>("Game Info").maxLevel;

            Application.targetFrameRate = targetFps;
            // DOTween.SetTweensCapacity(2250, 1500);
            CurrentLevelInfo = FileUtilities.GetCurrentLevelInfo(MaxLevel);

            gameStatus = GameStatus.Menu;
        }

        private void Start()
        {
            HandleFirstTime();
        }

        private void HandleFirstTime()
        {
            if (PlayerPrefs.GetInt("first_time", 0) != 0) return;

            onGameFirstTime.Raise();
            PlayerPrefs.SetInt("first_time", 1);
        }

        public static void GameEnded(bool success, GameEndSettings gameEndSettings = GameEndSettings.EarlyStatusChange | GameEndSettings.EarlyActionInvoke, float? delay = null)
        {
            var toStatus = success ? GameStatus.Success : GameStatus.Fail;

            var eventToInvoke = success ? Instance.onGameSuccess : Instance.onGameFailed;

            var earlyStatusChange = gameEndSettings.IsFlagSet(GameEndSettings.EarlyStatusChange);
            var earlyUIChange = gameEndSettings.IsFlagSet(GameEndSettings.EarlyUIChange);
            var earlyActionInvoke = gameEndSettings.IsFlagSet(GameEndSettings.EarlyActionInvoke);

            CheckGameEndedSettings(true);

            if (Instance._levelEndedTween.IsActiveNPlaying()) return;

            Instance._levelEndedTween = DOVirtual.DelayedCall(delay ?? Instance.defaultDelayBeforeEnd, () =>
            {
                if (success)
                {
                    CinemachineManager.StopShake();
                    CalculateAndSetNextLevel();
                }

                CheckGameEndedSettings(false);
            });

            void InvokeEvents()
            {
                eventToInvoke.Invoke();
                Instance.onGameEnded.Invoke();
            }

            void ShowUI()
            {
                if (success) UIManager.Instance.Success();
                else UIManager.Instance.Fail();
            }

            void CheckGameEndedSettings(bool status)
            {
                if (earlyStatusChange == status) gameStatus = toStatus;
                if (earlyActionInvoke == status) InvokeEvents();
                if (earlyUIChange == status) ShowUI();
            }
        }

        private static void CalculateAndSetNextLevel()
        {
            var nextLevelSc = PlayerPrefs.GetInt("next_levelSc", 1);
            var nextLevelScFoo = nextLevelSc + 1;
            if (nextLevelSc > MaxLevel)
                nextLevelScFoo = 1;

            PlayerPrefs.SetInt("next_levelSc", nextLevelScFoo);

            PlayerPrefs.SetInt("next_level", PlayerPrefs.GetInt("next_level", 1) + 1);
        }

        public void NextLevel() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        public void RestartLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void StartGame()
        {
            gameStatus = GameStatus.Play;
            onGameStarted?.Invoke();
        }

        private void OnDestroy() => DOTween.KillAll();


        [Flags]
        public enum GameEndSettings
        {
            None = 0,
            EarlyActionInvoke = 1,
            EarlyUIChange = 2,
            EarlyStatusChange = 4
        }
    }

    [Flags]
    public enum GameStatus
    {
        None = 0,
        Menu = 1,
        Play = 2,
        Fail = 4,
        Success = 8,
        Deadlocked = 16
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(GameManager)), CanEditMultipleObjects]
    public class GameManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // var gameManager = target as GameManager;

            var guiStyle = new GUIStyle
            {
                richText = true
            };

            GUILayout.Label(
                Application.isPlaying ? $"<color=white><size=10> Current Status = </size></color><color=red>{GameManager.gameStatus}</color>" : "<color=white> Editor has to be in play mode in order to show current status </color>",
                guiStyle);

            GUILayout.Space(5);

            DrawDefaultInspector();
        }
    }
#endif
}