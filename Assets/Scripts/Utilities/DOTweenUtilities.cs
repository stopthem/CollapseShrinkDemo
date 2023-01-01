using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace CanTemplate.Utilities
{
    public static class DOTweenUtilities
    {
        public static Tween CallbackWhileWait(float duration, TweenCallback<float> onUpdate, TweenCallback onCompleted = null)
        {
            var t = DOVirtual.DelayedCall(duration, onCompleted);

            t.OnUpdate(() => onUpdate.Invoke(t.ElapsedPercentage()));

            return t;
        }

        public static Tween LoopWait<T>(List<T> list, float delayBetweenElements, System.Action<T, int, bool> elementAction, TweenCallback onIterateEnd = null, bool ignoreTimescale = true) =>
            LoopWait(list.Count, delayBetweenElements, (i, isLast) => elementAction.Invoke(list.ElementAt(i), i, isLast), onIterateEnd, ignoreTimescale);

        public static Tween LoopWait(int count, float delayBetweenElements, System.Action<int, bool> elementAction, TweenCallback onIterateEnd = null, bool ignoreTimescale = true)
        {
            var loopCount = 0;

            var t = DOVirtual.DelayedCall(delayBetweenElements, () => { }, ignoreTimescale).SetLoops(count);

            t.OnStepComplete(() =>
            {
                var isLast = loopCount == count - 1;
                elementAction.Invoke(loopCount, isLast);

                if (isLast)
                {
                    onIterateEnd?.Invoke();
                }

                loopCount++;
            });

            return t;
        }
    }
}