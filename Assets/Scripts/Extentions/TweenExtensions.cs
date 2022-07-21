using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using System.Linq;
using CanTemplate.Utilities;

namespace CanTemplate.Extensions
{
    public static class TweenExtensions
    {
        ///<summary>Moves and rotates target transform to given transform in world space.
        /// <para>Also sets the returned sequences target to given transform for filtering operations.</para>
        /// </summary>
        public static Sequence DoMoveRotate(this Transform t, Transform to, float duration, Ease ease = Ease.OutQuad)
        {
            var toPos = to.position;
            var toRotation = to.rotation.ClampEuler();
            return DOTween.Sequence()
                          .SetTarget(t)
                          .Append(t.DOMove(toPos, duration)
                                   .SetEase(ease))
                          .Join(t.DORotate(toRotation, duration)
                                 .SetEase(ease));
        }

        ///<summary>Moves and rotates target transform to given transform in local space.
        /// <para>Also sets the returned sequences target to given transform for filtering operations.</para>
        /// </summary>
        public static Sequence DoLocalMoveRotate(this Transform t, Transform to, float duration, Ease ease = Ease.OutQuad)
        {
            var toPos = to.localPosition;
            var toRotation = to.localRotation.ClampEuler();
            return DOTween.Sequence()
                          .SetTarget(t)
                          .Append(t.DOLocalMove(toPos, duration)
                                   .SetEase(ease))
                          .Join(t.DOLocalRotate(toRotation, duration)
                                 .SetEase(ease));
        }

        ///<summary>Sets given loopType with amount of <see langword ="loopAmount"/> and given <see langword ="delayBetweenLoops"/> between loops to target tween and returns a Sequence.
        ///<para>Also sets the returned sequences target to given transform for filtering operations.</para>
        /// </summary>
        public static Sequence SetLoopsWithDelay(this Tween t, float delayBetweenLoops, int loopAmount = 2, LoopType loopType = LoopType.Yoyo, bool delayAtStart = false)
        {
            var seq = DOTween.Sequence();
            if (delayAtStart) seq.AppendInterval(delayBetweenLoops);

            seq.Append(t)
               .AppendInterval(delayBetweenLoops)
               .SetLoops(loopAmount, loopType)
               .SetTarget(t.target);
            return seq;
        }

        public static Tween SetEase(this Tween t, EaseSelection easeSelection)
        {
            var selection = easeSelection.GetEase();

            if (selection.GetType() == typeof(Ease))
                t.SetEase((Ease)selection);
            else
                t.SetEase((AnimationCurve)selection);

            return t;
        }

        ///<summary>Fires the given callback at a given normalized ( 0-1(tween duration) ) <see langword="time"/>
        ///<para>Be aware that time is manipulated by Ease of this tween. Example = If you have DOTween.To from 0 to 100 with Ease of OutSine(default), time of a .5f will be around 70</para>
        /// </summary> 
        ///<param name = "time">Give a normalized time(0-1) between 0 and tween duration.</param>
        /// <param name="ignoreTimeScale">Should DOTween ignore Time.Timescale when waiting ?</param>
        public static Tween CallbackAtTime(this Tween t, float time, TweenCallback callback, bool includeLoops = true, bool ignoreTimeScale = false)
        {
            var callBackTween = DOVirtual.DelayedCall(t.Duration(includeLoops) * time, callback, ignoreTimeScale);
            t.OnKill(() => callBackTween.Kill());

            return t;
        }

        ///<summary>Fires the given callback at a given normalized ( 0-1(tween duration) ) <see langword="time"/>
        ///<para>Be aware that time is manipulated by Ease of this tween. Example = If you have DOTween.To from 0 to 100 with Ease of OutSine(default), time of a .5f will be around 70</para>
        /// </summary> 
        ///<param name = "time">Give a normalized time(0-1) between 0 and tween duration.</param>
        /// <param name="ignoreTimeScale">Should DOTween ignore Time.Timescale when waiting ?</param>
        public static Tween CallbackAtTimeSpeedBased(this Tween t, float time, TweenCallback callback, bool includeLoops = true, bool ignoreTimeScale = false)
        {
            WaitUtilities.WaitForAFrame(() =>
            {
                var callBackTween = DOVirtual.DelayedCall(t.Duration(includeLoops) * time, callback, ignoreTimeScale);
                t.OnKill(() => callBackTween.Kill());
            });

            return t;
        }
        
        public static bool IsActiveNPlaying(this Tween t) => t.IsActive() && t.IsPlaying();
    }
}