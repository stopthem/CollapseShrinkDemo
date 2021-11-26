using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LerpManager : Singleton<LerpManager>
{
    [SerializeField] private AnimationCurve[] animationCurves;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        var instance = GameObject.FindGameObjectWithTag("LerpManager");
        if (instance && instance != gameObject)
        {
            Destroy(gameObject);
        }

        SceneManager.activeSceneChanged += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene current, Scene next)
    {
        if (this != null) StopAllCoroutines();
    }

    #region floatLerp
    public static void LerpOverTime(float value, float toValue, float speed, System.Action<float> valueAction,
    PresetAnimationCurves curve = PresetAnimationCurves.DECREASING, AnimationCurve overrideCurve = null, float timeActionWhen = 0, System.Action timeAction = null,
     float delayBeforeAction = 0, System.Action normalAction = null,
       bool delayTillTap = false)
    {
        AnimationCurve actualCurve = Instance.animationCurves[(int)curve];
        if (overrideCurve != null)
        {
            actualCurve = overrideCurve;
        }
        Instance.StartCoroutine(Instance.LerpRoutine(value, toValue, speed, actualCurve, valueAction, normalAction, timeActionWhen, timeAction, delayBeforeAction, delayTillTap));
    }

    public static void LerpOverTime(float value, float toValue, float speed, System.Action<float> valueAction,
    out IEnumerator coroutine, PresetAnimationCurves curve = PresetAnimationCurves.DECREASING, AnimationCurve overrideCurve = null, float timeActionWhen = 0, System.Action timeAction = null,
     float delayBeforeAction = 0, System.Action normalAction = null,
       bool delayTillTap = false)
    {
        AnimationCurve actualCurve = Instance.animationCurves[(int)curve];
        if (overrideCurve != null)
        {
            actualCurve = overrideCurve;
        }
        coroutine = Instance.LerpRoutine(value, toValue, speed, actualCurve, valueAction, normalAction, timeActionWhen, timeAction, delayBeforeAction, delayTillTap);
        Instance.StartCoroutine(coroutine);
    }
    #endregion

    #region vectorLerp
    public static void LerpOverTime(Vector3 value, Vector3 toValue, float speed, System.Action<Vector3> valueAction,
     PresetAnimationCurves curve = PresetAnimationCurves.DECREASING, AnimationCurve overrideCurve = null, float timeActionWhen = 0, System.Action timeAction = null,
      float delayBeforeAction = 0, System.Action normalAction = null, bool delayTillTap = false)
    {
        AnimationCurve actualCurve = Instance.animationCurves[(int)curve];
        if (overrideCurve != null)
        {
            actualCurve = overrideCurve;
        }
        Instance.StartCoroutine(Instance.LerpRoutine(value, toValue, speed, actualCurve, valueAction, normalAction, timeActionWhen, timeAction, delayBeforeAction, delayTillTap));
    }

    public static void LerpOverTime(Vector3 value, Vector3 toValue, float speed, System.Action<Vector3> valueAction,
     out IEnumerator coroutine, PresetAnimationCurves curve = PresetAnimationCurves.DECREASING, AnimationCurve overrideCurve = null, float timeActionWhen = 0, System.Action timeAction = null,
     float delayBeforeAction = 0, System.Action normalAction = null,
       bool delayTillTap = false)
    {
        AnimationCurve actualCurve = Instance.animationCurves[(int)curve];
        if (overrideCurve != null)
        {
            actualCurve = overrideCurve;
        }
        coroutine = Instance.LerpRoutine(value, toValue, speed, actualCurve, valueAction, normalAction, timeActionWhen, timeAction, delayBeforeAction, delayTillTap);
        Instance.StartCoroutine(coroutine);
    }
    #endregion

    #region quartenionLerp
    public static void LerpOverTime(Quaternion value, Quaternion toValue, float speed, System.Action<Quaternion> valueAction,
    PresetAnimationCurves curve = PresetAnimationCurves.DECREASING, AnimationCurve overrideCurve = null, float timeActionWhen = 0, System.Action timeAction = null,
     float delayBeforeAction = 0, System.Action normalAction = null, bool delayTillTap = false)
    {
        AnimationCurve actualCurve = Instance.animationCurves[(int)curve];
        if (overrideCurve != null)
        {
            actualCurve = overrideCurve;
        }
        Instance.StartCoroutine(Instance.LerpRoutine(value, toValue, speed, actualCurve, valueAction, normalAction, timeActionWhen, timeAction, delayBeforeAction, delayTillTap));
    }

    public static void LerpOverTime(Quaternion value, Quaternion toValue, float speed, System.Action<Quaternion> valueAction,
    out IEnumerator coroutine, PresetAnimationCurves curve = PresetAnimationCurves.DECREASING, AnimationCurve overrideCurve = null, float timeActionWhen = 0, System.Action timeAction = null,
     float delayBeforeAction = 0, System.Action normalAction = null,
       bool delayTillTap = false)
    {
        AnimationCurve actualCurve = Instance.animationCurves[(int)curve];
        if (overrideCurve != null)
        {
            actualCurve = overrideCurve;
        }
        coroutine = Instance.LerpRoutine(value, toValue, speed, actualCurve, valueAction, normalAction, timeActionWhen, timeAction, delayBeforeAction, delayTillTap);
        Instance.StartCoroutine(coroutine);
    }

    #endregion

    public static void WaitForFrames(int howManyFrames, System.Action action) => Instance.StartCoroutine(Instance.WaitForFramesRoutine(howManyFrames, action));

    private IEnumerator WaitForFramesRoutine(int howManyFrames, System.Action action)
    {
        for (int i = 0; i < howManyFrames; i++)
        {
            yield return null;
        }
        action.Invoke();
    }

    public static void LoopWait(int length, float timeBetweenElements, System.Action<int> everyElementAction, System.Action action = null)
    {
        Instance.StartCoroutine(Instance.LoopWaitRoutine(length, timeBetweenElements, everyElementAction, action));
    }

    private IEnumerator LoopWaitRoutine(int length, float timeBetweenElements, System.Action<int> everyElementAction, System.Action action)
    {
        for (int i = 0; i < length; i++)
        {
            everyElementAction.Invoke(i);
            yield return new WaitForSeconds(timeBetweenElements);
        }
        action?.Invoke();
    }

    public static void Wait(float waitTime, System.Action action, bool real = false) => Instance.StartCoroutine(Instance.WaitRoutine(waitTime, action, real));

    private IEnumerator WaitRoutine(float delay, System.Action action, bool real = false)
    {
        if (real) yield return new WaitForSecondsRealtime(delay);
        else yield return new WaitForSeconds(delay);
        action.Invoke();
    }

    private IEnumerator LerpRoutine(float value, float toValue, float speed, AnimationCurve curve, System.Action<float> valueAction,
     System.Action normalAction, float timeActionWhen, System.Action timeAction, float delayBeforeAction, bool delayTillTap)
    {
        var timer = 0f;
        float startValue = value;
        bool timeActionflag = false;
        while (timer < 1.0f)
        {
            if (timer > timeActionWhen && !timeActionflag)
            {
                timeActionflag = true;
                timeAction?.Invoke();
            }
            timer += Time.deltaTime * speed;
            valueAction.Invoke(Mathf.Lerp(startValue, toValue, curve.Evaluate(timer)));
            yield return null;
        }
        yield return new WaitForSeconds(delayBeforeAction);
        if (delayTillTap)
        {
            while (!Input.GetMouseButtonDown(0))
            {
                yield return null;
            }
        }
        normalAction?.Invoke();
    }

    private IEnumerator LerpRoutine(Vector3 value, Vector3 toValue, float speed, AnimationCurve curve, System.Action<Vector3> valueAction,
     System.Action normalAction, float timeActionWhen, System.Action timeAction, float delayBeforeAction, bool delayTillTap)
    {
        var timer = 0f;
        Vector3 startValue = value;
        bool timeActionflag = false;
        while (timer < 1.0f)
        {
            if (timer > timeActionWhen && !timeActionflag)
            {
                timeActionflag = true;
                timeAction?.Invoke();
            }
            timer += Time.deltaTime * speed;
            valueAction.Invoke(Vector3.Lerp(startValue, toValue, curve.Evaluate(timer)));
            yield return null;
        }
        yield return new WaitForSeconds(delayBeforeAction);
        if (delayTillTap)
        {
            while (!Input.GetMouseButtonDown(0))
            {
                yield return null;
            }
        }
        normalAction?.Invoke();
    }
    private IEnumerator LerpRoutine(Quaternion value, Quaternion toValue, float speed, AnimationCurve curve, System.Action<Quaternion> valueAction,
     System.Action normalAction, float timeActionWhen, System.Action timeAction, float delayBeforeAction, bool delayTillTap)
    {
        var timer = 0f;
        Quaternion startValue = value;
        bool timeActionflag = false;
        while (timer < 1.0f)
        {
            if (timer > timeActionWhen && !timeActionflag)
            {
                timeActionflag = true;
                timeAction?.Invoke();
            }
            timer += Time.deltaTime * speed;
            valueAction.Invoke(Quaternion.Lerp(startValue, toValue, curve.Evaluate(timer)));
            yield return null;
        }
        yield return new WaitForSeconds(delayBeforeAction);
        if (delayTillTap)
        {
            while (!Input.GetMouseButtonDown(0))
            {
                yield return null;
            }
        }
        normalAction?.Invoke();
    }

    public static AnimationCurve PresetToAnimationCurve(PresetAnimationCurves curve) => Instance.animationCurves[(int)curve];

    public static AnimationCurve CreateAnimationCurve(Keyframe start, Keyframe end, Keyframe[] extraKeys = null)
    {
        List<Keyframe> ks = new List<Keyframe>();
        ks.Add(start);
        ks.AddRange(extraKeys);
        if (ks.FirstOrDefault(x => x.time == 1).time == 0) ks.Add(end);
        return new AnimationCurve(ks.ToArray());
    }
}
public enum PresetAnimationCurves
{
    DECREASING,
    INCREASE,
    SMOOTH,
    LINEER,
    BOUNCE,
}
