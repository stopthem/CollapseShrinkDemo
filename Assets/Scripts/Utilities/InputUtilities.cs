using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace CanTemplate.Utils
{
    public static class InputUtilities
    {
        ///<summary>Tries to detect a click without dragging between mousebuttondown and mousebuttonup.</summary>
        ///<param name="checkAfterElapsed">Waits this amount of time after first click or when this function has been called to return a conculusion.</param>
        public static void ClickWithoutDrag(System.Action onSuccessfulClick, float waitTime = .175f, float checkAfterElapsed = .75f, bool requireInput = true)
        {
            if (Input.GetMouseButtonDown(0) && requireInput)
                StartClickWithoutDrag(onSuccessfulClick, waitTime, checkAfterElapsed);
            else if (!requireInput)
                StartClickWithoutDrag(onSuccessfulClick, waitTime, checkAfterElapsed);
        }

        private static void StartClickWithoutDrag(Action onFinished, float waitTime, float checkAfterElapsed)
        {
            var waitTween = DOVirtual.DelayedCall(waitTime, () => { });

            waitTween.OnUpdate(() =>
            {
                if (Input.GetMouseButtonUp(0))
                {
                    onFinished.Invoke();
                    waitTween.Kill();
                }
                else if (Input.GetMouseButton(0) && waitTween.ElapsedPercentage() > checkAfterElapsed) waitTween.Kill();
            });
        }
    }
}
