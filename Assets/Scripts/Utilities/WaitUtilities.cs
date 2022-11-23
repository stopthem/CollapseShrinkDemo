using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CanTemplate.Utilities
{
    public static class WaitUtilities
    {
        public static void WaitForAFrame(Action action) => CoroutineHelper.StartStaticCoroutine(WaitForFramesRoutine(1, action));

        public static void WaitForFrames(int howManyFrames, Action action) => CoroutineHelper.StartStaticCoroutine(WaitForFramesRoutine(howManyFrames, action));

        private static IEnumerator WaitForFramesRoutine(int howManyFrames, Action action)
        {
            for (int i = 0; i < howManyFrames; i++)
                yield return null;
            action.Invoke();
        }
    }
}