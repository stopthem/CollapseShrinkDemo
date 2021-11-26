using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Linq;
using UnityEngine.Events;
using CanTemplate.Utils;

public class GameManager : Singleton<GameManager>
{
    public static TimeManager timeManager;

    [HideInInspector] public static GameStatus gameStatus;

    public UnityEvent OnGameStarted, OnGameSuccess, OnGameFailed, OnGameEnded;

    public static LevelInfo CurrentLevelInfo;

    public static int CurrentLevelCount { get => PlayerPrefs.GetInt("next_level", 1); }

    private void Awake()
    {
        Application.targetFrameRate = 60;
        // DOTween.SetTweensCapacity(500, 125);
        CurrentLevelInfo = FileUtils.GetCurrentLevelInfo();
    }

    private void Start()
    {
        gameStatus = GameStatus.MENU;
    }

    public static void Fail(float delay = 0)
    {
        DOVirtual.DelayedCall(delay, () =>
         {
             Instance.OnGameFailed?.Invoke();
             Instance.OnGameEnded?.Invoke();
             gameStatus = GameStatus.FAIL;
         });
    }

    public static void Success(float delay = 0)
    {
        DOVirtual.DelayedCall(delay, () =>
        {
            int nextLevelSc = PlayerPrefs.GetInt("next_levelSc", 1);

            int nextLevelScFoo = nextLevelSc + 1;

            if (nextLevelSc == PlayerPrefs.GetInt("max_level")) nextLevelScFoo = 1;

            PlayerPrefs.SetInt("next_levelSc", nextLevelScFoo);

            PlayerPrefs.SetInt("next_level", PlayerPrefs.GetInt("next_level", 1) + 1);

            Instance.OnGameSuccess?.Invoke();
            Instance.OnGameEnded?.Invoke();

            gameStatus = GameStatus.SUCCESS;
        });
    }

    public void NextLevel() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    public void RestartLevel() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    public void StartGame()
    {
        Time.timeScale = 1;
        gameStatus = GameStatus.PLAY;
        OnGameStarted?.Invoke();
    }

    private void OnDestroy() => DOTween.KillAll();

    public enum GameStatus
    {
        MENU,
        PLAY,
        FAIL,
        SUCCESS,
    }
}