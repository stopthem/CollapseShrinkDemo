using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

namespace CanTemplate.Extensions
{
    public static class TweenExtensions
    {
        ///<summary>Moves and rotates target transform to given transform in world space.</summary>
        public static Sequence DoMoveRotate(this Transform t, Transform to, float duration, EaseSelection ease = null)
        {
            EaseSelection tweenEase = ease == null ? new EaseSelection(Ease.InOutQuad) : ease;

            Vector3 toPos = to.position;
            Vector3 toRotation = to.rotation.ClampEuler();
            Sequence seq = DOTween.Sequence();
            seq.SetTarget(t);
            return seq
            .Append(t.DOMove(toPos, duration).SetEase(tweenEase))
            .Join(t.DORotate(toRotation, duration).SetEase(tweenEase));
        }

        ///<summary>Moves and rotates target transform to given transform in local space.</summary>
        public static Sequence DoLocalMoveRotate(this Transform t, Transform to, float duration, EaseSelection ease = null)
        {
            EaseSelection tweenEase = ease == null ? new EaseSelection(Ease.InOutQuad) : ease;

            Vector3 toPos = to.localPosition;
            Vector3 toRotation = to.localRotation.ClampEuler();
            Sequence seq = DOTween.Sequence();
            seq.SetTarget(t);
            return seq
            .Append(t.DOLocalMove(toPos, duration).SetEase(tweenEase))
            .Join(t.DOLocalRotate(toRotation, duration).SetEase(tweenEase));
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

        ///<summary>Generates a tween callback at a given normalized time.</summary> 
        ///<param name = "time">Input a normalized time(0-1) between 0 and tween duration.</param>
        ///<param name = "includeLoops">Should DOTween calculate duration based on all loops(default) or just one loop.</param>
        public static Tween CallbackAtTime(this Tween t, float time, TweenCallback callback, bool includeLoops = true)
        {
            time = Mathf.Clamp01(time);
            DOVirtual.DelayedCall(t.Duration(includeLoops) * time, callback);
            return t;
        }
    }
}
