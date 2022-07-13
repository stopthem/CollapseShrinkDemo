using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

namespace CanTemplate.Extensions
{
    public static class TweenExtensions
    {
        ///<summary>Moves and rotates target transform to given transform in world space.
        /// <para>Also sets the returned sequences target to given transform for filtering operations.</para>
        /// </summary>
        public static Sequence DoMoveRotate(this Transform t, Transform to, float duration, EaseSelection ease = null)
        {
            EaseSelection tweenEase = ease ?? new EaseSelection(Ease.InOutSine);

            Vector3 toPos = to.position;
            Vector3 toRotation = to.rotation.ClampEuler();
            Sequence seq = DOTween.Sequence();
            seq.SetTarget(t);
            return seq
                .Append(t.DOMove(toPos, duration).SetEase(tweenEase))
                .Join(t.DORotate(toRotation, duration).SetEase(tweenEase));
        }

        ///<summary>Moves and rotates target transform to given transform in local space.
        /// <para>Also sets the returned sequences target to given transform for filtering operations.</para>
        /// </summary>
        public static Sequence DoLocalMoveRotate(this Transform t, Transform to, float duration, EaseSelection ease = null)
        {
            var tweenEase = ease ?? new EaseSelection(Ease.InOutSine);

            Vector3 toPos = to.localPosition;
            Vector3 toRotation = to.localRotation.ClampEuler();
            Sequence seq = DOTween.Sequence();
            seq.SetTarget(t);
            return seq
                .Append(t.DOLocalMove(toPos, duration).SetEase(tweenEase))
                .Join(t.DOLocalRotate(toRotation, duration).SetEase(tweenEase));
        }

        ///<summary>Sets given loopType with amount of <see langword ="loopAmount"/> and given <see langword ="delayBetweenLoops"/> between loops to target tween and returns a Sequence.
        ///<para>Note : If target is tween and not a sequence, preferably it must be written last as a option because it returns a sequence.</para>
        ///<para>Also sets the returned sequences target to given transform for filtering operations.</para>
        /// </summary>
        public static Sequence SetLoopsWithDelay(this Tween t, float delayBetweenLoops, int loopAmount = 2, LoopType loopType = LoopType.Yoyo, bool delayAtStart = false)
        {
            var seq = DOTween.Sequence();
            if (delayAtStart) seq.AppendInterval(delayBetweenLoops);
            
            seq.Append(t)
                .AppendInterval(delayBetweenLoops)
                .SetLoops(loopAmount, loopType);
            
            seq.SetTarget(t.target);
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

        ///<summary>Fires the given callback at a given normalized ( 0-1(tween duration) ) <see langword="time"/>.
        ///<para>Note : If you want to pause the editor and <see langword="ignoreTimeScale"/> is true, be aware that the pausing the editor causes a inaccurate timed callback
        ///(Editor pausing effects other DOTween things too if they are Time.Timescale independent(most are)). It will work fine without pausing.</para></summary> 
        ///<param name = "time">Give a normalized time(0-1) between 0 and tween duration.</param>
        ///<param name = "includeLoops">Should DOTween calculate duration based on all loops(default) or just one loop.</param>
        ///<param name = "ignoreTimeScale">Should DOTween ignore time scale when calculating duration ?</param>
        public static Tween CallbackAtTime(this Tween t, float time, TweenCallback callback, bool includeLoops = true, bool ignoreTimeScale = false)
        {
            time = Mathf.Clamp01(time);
            DOVirtual.DelayedCall(t.Duration(includeLoops) * time, callback, ignoreTimeScale);
            return t;
        }
    }
}