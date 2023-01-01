using System;
using System.Collections;
using System.Collections.Generic;
using CanTemplate.Extensions;
using DG.Tweening;
using UnityEngine;

namespace CanTemplate
{
    [Serializable]
    public class TweenOptions
    {
        public float duration;
        public EaseSelection easeSelection;
        public Tween myTween;
    }

    [Serializable]
    public class FTweenOptions : TweenOptions
    {
        public float target;
    }

    [Serializable]
    public class TweenOptionsDEase : TweenOptions
    {
        public EaseSelection secondEaseSelection;
    }
}