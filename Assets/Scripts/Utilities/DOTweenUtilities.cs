using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using CanTemplate.Extensions;
using DG.Tweening;
using UnityEngine;

namespace CanTemplate.Utilities
{
    public static class DOTweenUtilities
    {
        public static Tween CallbackWhileWait(float duration, TweenCallback onUpdate, TweenCallback onCompleted = null) => DOVirtual.DelayedCall(duration, onCompleted).OnUpdate(onUpdate);

        public static void LoopWait<T>(List<T> list, float delayBetweenElements, System.Action<T, int, bool> elementAction, bool ignoreTimescale = true)
        {
            var loopCount = 0;

            var t = DOVirtual.DelayedCall(delayBetweenElements, () => { Debug.Log("loop waiting step complete"); }, ignoreTimescale).SetLoops(list.Count);

            t.OnStepComplete(() =>
            {
                var element = list.ElementAt(loopCount);
                elementAction.Invoke(element, loopCount, loopCount == list.Count - 1);
                loopCount++;
            });
        }

        public static Tween LoopWait(int count, float delayBetweenElements, System.Action<int, bool> elementAction, bool ignoreTimescale = true)
        {
            var loopCount = 0;

            return DOVirtual.DelayedCall(delayBetweenElements, () =>
            {
                elementAction.Invoke(loopCount, loopCount == count - 1);
                loopCount++;
            }, ignoreTimescale).SetLoops(count);
        }
    }
}