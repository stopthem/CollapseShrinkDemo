using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Linq;
using UnityEngine.Events;
using CanTemplate.Utils;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private int targetFps;
    [SerializeField] private float delayBeforeGameEnd = .75f;
    [HideInInspector] public static GameStatus gameStatus;

    public UnityEvent onGameStarted, onGameSuccess, onGameFailed, onGameEnded;

    public static LevelInfo currentLevelInfo;

    public static int CurrentLevelCount => PlayerPrefs.GetInt("next_level", 1);

    public static int maxLevel;

    private void Awake()
    {
        instance = this;
        maxLevel = Resources.Load<GameInfo>("Game Info").maxLevel;

        Application.targetFrameRate = targetFps;
        // DOTween.SetTweensCapacity(500, 125);
        currentLevelInfo = FileUtils.GetCurrentLevelInfo(maxLevel);
    }

    private void Start()
    {
        gameStatus = GameStatus.Menu;
    }

    /// <param name="delay">If 0, GameManager's <see cref="delayBeforeGameEnd"/> will be used.</param>
    public static void Fail(float delay = 0) => GameEnded(GameStatus.Fail, instance.onGameFailed, delay);

    private static void GameEnded(GameStatus toStatus, UnityEvent eventToInvoke, float delay = 0)
    {
        if (gameStatus == toStatus) return;

        DOVirtual.DelayedCall(delay, () =>
        {
            CalculateAndSetNextLevel();

            instance.onGameSuccess.Invoke();
            eventToInvoke.Invoke();

            gameStatus = toStatus;
        });
    }

    /// <param name="delay">If 0, GameManager's <see cref="delayBeforeGameEnd"/> will be used.</param>
    public static void Success(float delay = 0) => GameEnded(GameStatus.Success, instance.onGameSuccess, delay);

    private static void CalculateAndSetNextLevel()
    {
        var nextLevelScFoo = GetLevelScNumber();
        PlayerPrefs.SetInt("next_levelSc", nextLevelScFoo);

        PlayerPrefs.SetInt("next_level", PlayerPrefs.GetInt("next_level", 1) + 1);
    }

    public static int GetLevelScNumber()
    {
        var nextLevelSc = PlayerPrefs.GetInt("next_levelSc", 1);
        var nextLevelScFoo = nextLevelSc + 1;
        if (nextLevelSc > maxLevel)
            nextLevelScFoo = 1;
        return nextLevelScFoo;
    }

    public void NextLevel() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    public void RestartLevel() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    public void StartGame()
    {
        Time.timeScale = 1;
        gameStatus = GameStatus.Play;
        onGameStarted?.Invoke();
    }

    private void OnDestroy() => DOTween.KillAll();

    public enum GameStatus
    {
        Menu,
        Play,
        Fail,
        Success,
    }
}