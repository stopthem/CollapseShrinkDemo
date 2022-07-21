using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Linq;
using CanTemplate.Extensions;
using UnityEngine.Events;
using CanTemplate.Utilities;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private float defaultDelayBeforeEnd = .5f;
    [SerializeField] private int targetFps = 60;
    public static GameStatus gameStatus;
    
    public UnityEvent onGameStarted, onGameSuccess, onGameFailed, onGameEnded;

    public static LevelInfo CurrentLevelInfo { get; private set; }

    public static int CurrentLevelCount => PlayerPrefs.GetInt("next_level", 1);

    public static int MaxLevel { get; private set; }

    private Tween _levelEndedTween;

    private void Awake()
    {
        instance = this;
        MaxLevel = Resources.Load<GameInfo>("Game Info").maxLevel;

        Application.targetFrameRate = targetFps;
        // DOTween.SetTweensCapacity(500, 125);
        CurrentLevelInfo = FileUtilities.GetCurrentLevelInfo(MaxLevel);
    }

    private void Start()
    {
        gameStatus = GameStatus.Menu;
    }

    /// <param name="delay">If null, <see cref="defaultDelayBeforeEnd"/> will be used.</param>
    public static void Fail(float? delay = null, bool earlyStatusChange = true) => GameEnded(GameStatus.Fail, instance.onGameFailed, delay ?? instance.defaultDelayBeforeEnd, earlyStatusChange);

    private static void GameEnded(GameStatus toStatus, UnityEvent eventToInvoke, float delay = 0, bool earlyStatusChange = true)
    {
        // if (gameStatus is not GameStatus.Play) return;

        if (earlyStatusChange) gameStatus = toStatus;

        if (instance._levelEndedTween.IsActiveNPlaying()) return;

        instance._levelEndedTween = DOVirtual.DelayedCall(delay, () =>
        {
            if (gameStatus is not GameStatus.Play && !earlyStatusChange) return;

            if (toStatus is GameStatus.Success) CalculateAndSetNextLevel();

            eventToInvoke.Invoke();
            instance.onGameEnded.Invoke();
        });
    }

    /// <param name="delay">If null, <see cref="defaultDelayBeforeEnd"/> will be used.</param>
    public static void Success(float? delay = null, bool earlyStatusChange = true) => GameEnded(GameStatus.Success, instance.onGameSuccess, delay ?? instance.defaultDelayBeforeEnd, earlyStatusChange);

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

    public void RestartLevel() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

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
        Success
    }
}