using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace CanTemplate
{
    [Serializable]
    public class TweenOptions
    {
        public float duration;
        public Ease ease;
    }

    [Serializable]
    public class FTweenOptions : TweenOptions
    {
        public float target;
    }

    [Serializable]
    public class TweenOptionsDEase : TweenOptions
    {
        public Ease secondEase;
    }
}