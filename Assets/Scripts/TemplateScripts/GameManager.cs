using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [HideInInspector] public GameStatus gameStatus;
    public int levelNumber;

    public System.Action OnGameStarted;
    public System.Action OnGameSuccess;
    public System.Action OnGameFailed;

    [HideInInspector] public bool gameEnded;


    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        // Time.timeScale = 0;
    }


    private void Start()
    {
        gameStatus = GameStatus.MENU;
    }

    public void Fail()
    {
        OnGameFailed?.Invoke();
        gameStatus = GameStatus.FAIL;
        UIManager.Instance.Fail();
    }

    public void Success()
    {
        OnGameSuccess?.Invoke();
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

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void StartGame()
    {
        Time.timeScale = 1;
        gameStatus = GameStatus.PLAY;
        OnGameStarted?.Invoke();
    }

    public static void PlayParticle(GameObject expObj, Transform objTransform, Vector3 pos, Vector3 scale, bool doParent = false)
    {
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
                expObj.GetComponent<Poolable>().imTaken = false;
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
