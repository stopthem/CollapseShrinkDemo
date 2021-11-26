using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [HideInInspector] public static GameStatus gameStatus;
    public int levelNumber = 1;

    public static System.Action<bool> OnGameStartOrEnd;
    public static System.Action<bool> OnGameEnded;

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }


    private void Start() => gameStatus = GameStatus.MENU;

    public static void Fail()
    {
        OnGameStartOrEnd?.Invoke(false);
        OnGameEnded?.Invoke(false);
        gameStatus = GameStatus.FAIL;
        UIManager.Instance.Fail();
    }

    public static void Success()
    {
        OnGameEnded?.Invoke(true);
        OnGameStartOrEnd?.Invoke(false);
        gameStatus = GameStatus.SUCCESS;
        UIManager.Instance.Success();
    }

    public void NextLevel()
    {
        int nextLevelNum = levelNumber + 1;

        if (nextLevelNum == 2)
        {
            nextLevelNum = 1;
        }

        SceneManager.LoadScene("Level" + nextLevelNum);
    }

    public void RestartLevel() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    public void StartGame()
    {
        Time.timeScale = 1;
        gameStatus = GameStatus.PLAY;
        OnGameStartOrEnd?.Invoke(true);
    }

    public static void PlayParticle(GameObject expObj, Transform objTransform, Vector3 pos = default(Vector3), Vector3 scale = default(Vector3), bool doParent = false)
    {
        pos = pos == default(Vector3) ? objTransform.position : pos;
        scale = scale == default(Vector3) ? expObj.transform.localScale : scale;

        if (doParent) expObj.transform.parent = objTransform;

        Vector3 startScale = expObj.transform.localScale;
        var particle = expObj.GetComponent<ParticleSystem>();
        expObj.transform.position = pos;
        if (scale != Vector3.zero)
        {
            expObj.transform.localScale = scale;
        }
        particle.Play();
        var main = particle.main;
        LerpManager.Wait(main.duration, () =>
        {
            particle.Stop();
            if (expObj.GetComponent<Poolable>())
            {
                expObj.GetComponent<Poolable>().ClearMe();
                expObj.transform.localScale = startScale;
            }
            else
            {
                Destroy(expObj);
            }
        });
    }

    public enum GameStatus
    {
        MENU,
        PLAY,
        FAIL,
        SUCCESS,
    }
}
