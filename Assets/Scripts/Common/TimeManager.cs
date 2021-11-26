using UnityEngine;
using System.Collections;

public class TimeManager : MonoBehaviour
{
    private IEnumerator _waitRoutine;

    private void Awake()
    {
        GameManager.timeManager = this;
    }

    public void DoSlowMotion(float intensity, float time)
    {
        if (_waitRoutine != null) StopCoroutine(_waitRoutine);

        ResetTimes();

        _waitRoutine = WaitRoutine(intensity, time);
        StartCoroutine(_waitRoutine);
    }

    private static void ResetTimes()
    {
        Time.timeScale = 1;
        Time.fixedDeltaTime = Time.timeScale * .02f;
    }

    private IEnumerator WaitRoutine(float intensity, float time)
    {

        Time.timeScale = intensity;
        Time.fixedDeltaTime = Time.fixedDeltaTime * intensity;

        yield return new WaitForSecondsRealtime(time);

        ResetTimes();
    }
}