using System;
using System.Collections;
using System.Collections.Generic;
using CanTemplate;
using CanTemplate.Extensions;
using CanTemplate.GameManaging;
using CanTemplate.Utilities;
using DG.DeInspektor.Attributes;
using DG.Tweening;
using Dreamteck.Splines;
using UnityEngine;

namespace CanTemplate.DreamteckSpline
{
    [RequireComponent(typeof(SplineFollower))]
    public class SplineFollowerManager : MonoBehaviour
    {
        [SerializeField] private TweenOptions toSpeedTweenOptions = new()
        {
            duration = .75f,
            easeSelection = new EaseSelection(Ease.OutSine)
        };

        public SplineFollower SplineFollower { get; private set; }

        [SerializeField] private bool automaticStartAtOnGameStarted = true;

        public float OldSpeed { get; private set; }

        private void Awake()
        {
            SplineFollower = GetComponent<SplineFollower>();
        }

        private void Start()
        {
            if (automaticStartAtOnGameStarted)
            {
                GameManager.Instance.onGameStarted.AddListener(StartFollowing);
            }

            GameManager.Instance.onGameFailed.AddListener(() => StopOrGo(false));

            OldSpeed = SplineFollower.followSpeed;
            SplineFollower.followSpeed = 0;
        }

        public void StartFollowing()
        {
            StopOrGo(true);
            DOTween.To(value => SplineFollower.followSpeed = value, 0, OldSpeed, toSpeedTweenOptions.duration).SetEase(toSpeedTweenOptions.easeSelection);
        }

        public void ChangeSpline(SplineComputer splineComputer)
        {
            SplineFollower.spline = splineComputer;
            SplineFollower.RebuildImmediate();
            SplineFollower.SetPercent(0);
        }

        public void StopOrGo(bool status) => SplineFollower.follow = status;
    }
}