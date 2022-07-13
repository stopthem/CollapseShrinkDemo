using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LerpManager : Singleton<LerpManager>
{
    [SerializeField] private AnimationCurve[] animationCurves;

    public static void WaitForFrames(int howManyFrames, System.Action action) => Instance.StartCoroutine(Instance.WaitForFramesRoutine(howManyFrames, action));

    public static void WaitForAFrame(System.Action action) => Instance.StartCoroutine(Instance.WaitForFramesRoutine(1, action));

    private IEnumerator WaitForFramesRoutine(int howManyFrames, System.Action action)
    {
        for (int i = 0; i < howManyFrames; i++)
            yield return null;
        action.Invoke();
    }

    public static void LoopWait<T>(T[] array, float timeBetweenElements, System.Action<T, int> elementAction, System.Action finishedAction = null)
    {
        Instance.StartCoroutine(LoopWaitRoutine<T>(array, timeBetweenElements, elementAction, finishedAction));
    }

    private static IEnumerator LoopWaitRoutine<T>(T[] array, float timeBetweenElements, System.Action<T, int> elementAction, System.Action finishedAction = null)
    {
        for (int i = 0; i < array.Length; i++)
        {
            elementAction.Invoke(array[i], i);
            yield return new WaitForSeconds(timeBetweenElements);
        }
        finishedAction?.Invoke();
    }

    public static void LoopWait(int arrayLength, float timeBetweenElements, System.Action<int, bool> elementAction, System.Action finishedAction = null)
    {
        Instance.StartCoroutine(LoopWaitRoutine(arrayLength, timeBetweenElements, elementAction, finishedAction));
    }

    private static IEnumerator LoopWaitRoutine(int arrayLength, float timeBetweenElements, System.Action<int, bool> elementAction, System.Action finishedAction = null)
    {
        for (int i = 0; i < arrayLength; i++)
        {
            elementAction.Invoke(i, i == arrayLength - 1);
            yield return new WaitForSeconds(timeBetweenElements);
        }
        finishedAction?.Invoke();
    }

    public static AnimationCurve PresetToAnimationCurve(PresetAnimationCurves curve) => Instance.animationCurves[(int)curve];
}

public enum PresetAnimationCurves
{
    Out1Bounce,
    InTop1Out0Bounce
}
