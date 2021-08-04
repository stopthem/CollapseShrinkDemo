using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LerpManager : MonoBehaviour
{
    public static LerpManager instance;
    public enum Curves
    {
        DECREASING = 0,
        INCREASE = 1,
        SMOOTH = 2,
        LINEER = 3,
        UPNDOWN = 4
    }

    [SerializeField] private AnimationCurve[] animationCurves;

    private void Awake()
    {
        instance = this;
    }

    public static void Wait(float delay, System.Action action)
    {
        instance.StartCoroutine(instance.WaitRoutine(delay, action));
    }

    public static void LerpOverTime(float value, float toValue, float speed, System.Action<float> valueAction,
     AnimationCurve overrideCurve = null, Curves curve = Curves.DECREASING, System.Action normalAction = null,
      float timeActionWhen = 0, System.Action timeAction = null, float delayBeforeAction = 0, bool delayTillTap = false, float time = 1f)
    {
        AnimationCurve actualCurve = instance.animationCurves[(int)curve];
        if (overrideCurve != null)
        {
            actualCurve = overrideCurve;
        }
        instance.StartCoroutine(instance.LerpRoutine(value, toValue, speed, actualCurve, valueAction, normalAction, timeActionWhen, timeAction, delayBeforeAction, delayTillTap, time));
    }

    public static void LerpOverTime(Vector3 value, Vector3 toValue, float speed, System.Action<Vector3> valueAction,
     AnimationCurve overrideCurve = null, Curves curve = Curves.DECREASING, System.Action normalAction = null, float timeActionWhen = 0,
      System.Action timeAction = null, float delayBeforeAction = 0, bool delayTillTap = false, float time = 1f)
    {
        AnimationCurve actualCurve = instance.animationCurves[(int)curve];
        if (overrideCurve != null)
        {
            actualCurve = overrideCurve;
        }
        instance.StartCoroutine(instance.LerpRoutine(value, toValue, speed, actualCurve, valueAction, normalAction, timeActionWhen, timeAction, delayBeforeAction, delayTillTap, time));
    }
    public static void LerpOverTime(Quaternion value, Quaternion toValue, float speed, System.Action<Quaternion> valueAction,
     AnimationCurve overrideCurve = null, Curves curve = Curves.DECREASING, System.Action normalAction = null, float timeActionWhen = 0, System.Action timeAction = null, float delayBeforeAction = 0, float time = 1f)
    {
        AnimationCurve actualCurve = instance.animationCurves[(int)curve];
        if (overrideCurve != null)
        {
            actualCurve = overrideCurve;
        }
        instance.StartCoroutine(instance.LerpRoutine(value, toValue, speed, actualCurve, valueAction, normalAction, timeActionWhen, timeAction, delayBeforeAction, time));
    }

    public IEnumerator WaitRoutine(float delay, System.Action action)
    {
        yield return new WaitForSeconds(delay);
        action.Invoke();
    }

    public IEnumerator LerpRoutine(float value, float toValue, float speed, AnimationCurve curve, System.Action<float> valueAction,
     System.Action normalAction = null, float timeActionWhen = 0, System.Action timeAction = null, float delayBeforeAction = 0, bool delayTillTap = false, float time = 1f)
    {
        var timer = 0f;
        float startValue = value;
        bool timeActionflag = false;
        while (timer < time)
        {
            if (timer > timeActionWhen && !timeActionflag)
            {
                timeActionflag = true;
                timeAction?.Invoke();
            }
            timer += Time.deltaTime * speed;
            value = Mathf.Lerp(startValue, toValue, curve.Evaluate(timer));
            valueAction.Invoke(value);
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
     System.Action normalAction = null, float timeActionWhen = 0, System.Action timeAction = null, float delayBeforeAction = 0, bool delayTillTap = false, float time = 1f)
    {
        var timer = 0f;
        Vector3 startValue = value;
        bool timeActionflag = false;
        while (timer < time)
        {
            if (timer > timeActionWhen && !timeActionflag)
            {
                timeActionflag = true;
                timeAction?.Invoke();
            }
            timer += Time.deltaTime * speed;
            value = Vector3.Lerp(startValue, toValue, curve.Evaluate(timer));
            valueAction.Invoke(value);
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
     System.Action normalAction = null, float timeActionWhen = 0, System.Action timeAction = null, float delayBeforeAction = 0, float time = 1f)
    {
        var timer = 0f;
        Quaternion startValue = value;
        bool timeActionflag = false;
        while (timer < time)
        {
            if (timer > timeActionWhen && !timeActionflag)
            {
                timeActionflag = true;
                timeAction?.Invoke();
            }
            timer += Time.deltaTime * speed;
            value = Quaternion.Lerp(startValue, toValue, curve.Evaluate(timer));
            valueAction.Invoke(value);
            yield return null;
        }
        yield return new WaitForSeconds(delayBeforeAction);
        normalAction?.Invoke();
    }

    public static AnimationCurve ReturnCurve(Curves curve)
    {
        return instance.animationCurves[(int)curve];
    }
}
