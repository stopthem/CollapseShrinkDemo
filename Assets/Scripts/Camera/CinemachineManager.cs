using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
using CanTemplate.Extensions;
using CanTemplate.Input;
using CanTemplate.Utilities;
using JetBrains.Annotations;

namespace CanTemplate.Camera
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class CinemachineManager : MonoBehaviour
    {
        public static CinemachineManager Instance { get; private set; }

        public static UnityEngine.Camera MainCam { get; private set; }
        public static Transform MainCamTransform { get; private set; }
        public static Ray CamToTouchRay => MainCam.ScreenPointToRay(InputManager.GetLastTouchPos);

        public enum CinemachinePos
        {
            Player
        }

        #region Cinemachine Info and Offset Rotations

        [SerializeField] private CinemachinePos startingVirtualCamera;

        [SerializeField] private CinemachineInfo[] cinemachineInfos;

        private static CinemachineVirtualCamera _currentVirtualCamera;

        private static CinemachineInfo _currentCinemachineInfo;

        public static CinemachineInfo CurrentCinemachineInfo
        {
            get => _currentCinemachineInfo;
            set
            {
                _currentCinemachineInfo = value;
                _currentVirtualCamera = value.cinemachineVirtualCamera;
            }
        }

        [Space(5), SerializeField] private OffsetRotation[] offsetsRotations;
        private static CinemachinePos _currentCinemachinePos;

        #endregion

        #region Default Tween Params

        [Header("Default Tween Params"), Space(5), SerializeField]
        private TweenOptions defaultTweenOptions = new()
        {
            duration = .5f,
            easeSelection = new EaseSelection(Ease.OutSine)
        };

        #endregion

        #region Shake

        [Header("Shake"), Space(5), SerializeField]
        private Ease shakeFinishEase = Ease.OutSine;

        [SerializeField] private NoiseSettings shakeNoise;

        private Sequence _shakeSeq;

        #endregion

        public static CinemachineTransposer transposer;
        private static CinemachineBasicMultiChannelPerlin _multiChannelPerlin;
        private static CinemachineBrain _cinemachineBrain;

        private float _startFov;
        private Tween _fovTween;

        private void Awake()
        {
            Instance = this;
            MainCam = UnityEngine.Camera.main;
            MainCamTransform = MainCam!.transform;

            _cinemachineBrain = GetComponent<CinemachineBrain>();

            foreach (var vc in cinemachineInfos.Select(x => x.cinemachineVirtualCamera))
                vc.enabled = false;

            ChangeVirtualCamera(startingVirtualCamera, blend: false);
            if (_currentVirtualCamera) UpdateCinemachineVariables();
        }

        private static void UpdateCinemachineVariables()
        {
            transposer = _currentVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            _multiChannelPerlin = _currentVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }

        private void Start()
        {
            if (_currentVirtualCamera) _startFov = _currentVirtualCamera.m_Lens.FieldOfView;

            if (offsetsRotations.ElementAtOrDefault(0) != null)
            {
                transposer.m_FollowOffset = offsetsRotations[0].offset;
            }
        }

        public static void Shake(ShakeInfo shakeInfo, bool overridePreviousShake = true)
        {
            if (Instance._shakeSeq.IsActiveNPlaying())
            {
                if (overridePreviousShake) Instance._shakeSeq.Complete();
                else return;
            }


            _multiChannelPerlin = _currentVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            if (!_multiChannelPerlin)
            {
                Debug.Log(_currentVirtualCamera + " has no multi channel perlin!");
                return;
            }

            var oldAmplitudeGain = _multiChannelPerlin.m_AmplitudeGain;
            var oldNoise = _multiChannelPerlin.m_NoiseProfile;
            _multiChannelPerlin.m_NoiseProfile = Instance.shakeNoise;

            _multiChannelPerlin.m_AmplitudeGain = shakeInfo.intensity;

            Instance._shakeSeq = DOTween.Sequence()
                .Append(DOTween.To(x => _multiChannelPerlin.m_AmplitudeGain = x, shakeInfo.intensity, 0, shakeInfo.duration))
                .Append(DOTween.To(x => _multiChannelPerlin.m_AmplitudeGain = x, _multiChannelPerlin.m_AmplitudeGain, oldAmplitudeGain, shakeInfo.smoothFinishDuration)
                    .SetEase(Instance.shakeFinishEase))
                .SetUpdate(true)
                .OnComplete(BackToOldNoise)
                .OnKill(BackToOldNoise);

            void BackToOldNoise() => _multiChannelPerlin.m_NoiseProfile = oldNoise;
        }

        public static void StopShake()
        {
            Instance._shakeSeq.Complete();

            _multiChannelPerlin.m_AmplitudeGain = 0;
        }

        public void EmptyFollowTarget(bool emptyLookAt = false)
        {
            _currentVirtualCamera.m_Follow = null;
            _currentVirtualCamera.m_LookAt = emptyLookAt ? null : _currentVirtualCamera.m_LookAt;
        }

        public static void ChangeTargets([CanBeNull] CinemachineVirtualCamera cinemachineVirtualCamera = null, [CanBeNull] Transform follow = null, [CanBeNull] Transform lookAt = null)
        {
            var vc = cinemachineVirtualCamera ? cinemachineVirtualCamera : _currentVirtualCamera;
            if (follow) vc.m_Follow = follow;
            if (lookAt) vc.LookAt = lookAt;
        }

        public static void SetOffsetX(float toX) => transposer.m_FollowOffset = transposer.m_FollowOffset.WithX(toX);

        public static Tween MoveOnlyPos(CinemachinePos cinemachinePos, float? duration = null, Ease? ease = null)
        {
            _currentVirtualCamera.DOKill();

            return DOVirtual.Vector3(transposer.m_FollowOffset, Instance.offsetsRotations[(int)cinemachinePos].offset, GetDuration(duration), x => transposer.m_FollowOffset = x).SetEase(GetEase(ease))
                .SetTarget(_currentVirtualCamera)
                .OnComplete(() => _currentCinemachinePos = cinemachinePos);
        }

        #region Move Only Rotation

        /// <summary>
        /// Moves the camera's rotation to given offsetRotations cinemachinePos.
        /// </summary>
        /// <param name="cinemachinePos"></param>
        /// <param name="duration">If null, this scripts defaultDuration will be used</param>
        /// <param name="ease">If null, this scripts defaultEase will be used</param>
        /// <returns>The tween that moves the rotation</returns>
        public static Tween MoveOnlyRot(CinemachinePos cinemachinePos, float? duration = null, Ease? ease = null)
            => Instance.transform.DORotate(Instance.offsetsRotations[(int)cinemachinePos].rotation, GetDuration(duration)).SetEase(GetEase(ease))
                .SetTarget(_currentVirtualCamera)
                .OnComplete(() => _currentCinemachinePos = cinemachinePos);

        /// <summary>
        /// Moves the camera's rotation to given toRot.
        /// </summary>
        /// <param name="toRot"></param>
        /// <param name="duration">If null, this scripts defaultDuration will be used</param>
        /// <param name="ease">If null, this scripts defaultEase will be used</param>
        /// <returns>The tween that moves the rotation</returns>
        public static Tween MoveOnlyRot(Vector3 toRot, float? duration = null, Ease? ease = null)
            => Instance.transform.DORotate(toRot, GetDuration(duration)).SetEase(GetEase(ease))
                .SetTarget(_currentVirtualCamera);

        #endregion

        #region Move Position And Rotation

        /// <summary>
        /// Moves Cinemachine offset and rotation to given cinemachinePos.
        /// </summary>
        /// <param name="cinemachinePo"></param>
        /// <param name="duration">If null, this scripts defaultDuration will be used</param>
        /// <param name="ease">If null, this scripts defaultEase will be used</param>
        /// <returns>The sequence that moves the transposer offset and rotation</returns>
        public static Sequence Move(CinemachinePos cinemachinePo, float? duration = null, Ease? ease = null)
        {
            _currentVirtualCamera.DOKill();
            return DOTween.Sequence()
                .Append(DOVirtual.Vector3(transposer.m_FollowOffset, Instance.offsetsRotations[(int)cinemachinePo].offset, GetDuration(duration), x => transposer.m_FollowOffset = x).SetEase(GetEase(ease)))
                .Join(Instance.transform.DORotate(Instance.offsetsRotations[(int)cinemachinePo].rotation, GetDuration(duration)).SetEase(GetEase(ease)))
                .SetTarget(_currentVirtualCamera)
                .AppendCallback(() => _currentCinemachinePos = cinemachinePo);
        }

        #endregion

        private static EaseSelection GetEase(Ease? easeSelection) => easeSelection.HasValue ? new EaseSelection(easeSelection.Value) : Instance.defaultTweenOptions.easeSelection;

        private static float GetDuration(float? duration) => duration ?? Instance.defaultTweenOptions.duration;

        public static void ChangeUpdateMethod(CinemachineBrain.UpdateMethod updateMethod)
        {
            _cinemachineBrain.m_UpdateMethod = updateMethod;
            _cinemachineBrain.m_BlendUpdateMethod = CinemachineBrain.BrainUpdateMethod.FixedUpdate;
        }

        public static OffsetRotation GetOffsetRotation(CinemachinePos cinemachinePos) => Instance.offsetsRotations.First(x => x.cinemachinePos == cinemachinePos);
        public static CinemachineInfo GetCinemachineInfo(CinemachinePos cinemachinePos) => Instance.cinemachineInfos.First(x => x.cinemachinePos == cinemachinePos);

        public static void ChangeVirtualCamera(CinemachinePos toPos, CinemachineBlendDefinition? cinemachineBlendDefinition = null, bool blend = true, TweenCallback onBlendFinished = null)
        {
            Instance.StopAllCoroutines();

            var newVcInfo = Instance.cinemachineInfos.FirstOrDefault(x => x.cinemachinePos == toPos);

            if (newVcInfo == null || (CurrentCinemachineInfo?.cinemachineVirtualCamera && CurrentCinemachineInfo?.cinemachinePos == toPos)) return;

            if (CurrentCinemachineInfo?.cinemachineVirtualCamera) _currentVirtualCamera.enabled = false;

            var newVc = newVcInfo.cinemachineVirtualCamera;
            newVc.enabled = true;

            _cinemachineBrain.m_IgnoreTimeScale = newVcInfo.ignoreTimeScale;

            CurrentCinemachineInfo = newVcInfo;
            UpdateCinemachineVariables();

            if (!blend) return;

            var oldBlendDefinition = _cinemachineBrain.m_DefaultBlend;
            var newBlend = cinemachineBlendDefinition ?? oldBlendDefinition;
            _cinemachineBrain.m_DefaultBlend = newBlend;

            WaitUtilities.WaitForAFrame(
                () => Instance.StartCoroutine(BlendWaitCoroutine(() =>
                {
                    _cinemachineBrain.m_DefaultBlend = oldBlendDefinition;
                    onBlendFinished?.Invoke();
                })));
        }

        private static IEnumerator BlendWaitCoroutine(TweenCallback callback)
        {
            while (_cinemachineBrain.IsBlending)
            {
                yield return null;
            }

            callback.Invoke();
        }

        #region Field Of View

        public static Tween DoFov(float value, float duration, Ease? ease = null)
        {
            Instance._fovTween.Kill();
            _currentVirtualCamera.m_Lens.FieldOfView = Instance._startFov;
            return Instance._fovTween = DOTween.To(x => _currentVirtualCamera.m_Lens.FieldOfView = x, Instance._startFov, value, duration).SetEase(GetEase(ease));
        }

        public static void DoFov(float value, bool multiplier = false)
        {
            Instance._fovTween.Kill();
            _currentVirtualCamera.m_Lens.FieldOfView = multiplier ? Instance._startFov * value : value;
        }

        public static Tween DoMultiplierFov(float multiplier, float duration, Ease? ease = null)
        {
            Instance._fovTween.Kill();
            _currentVirtualCamera.m_Lens.FieldOfView = Instance._startFov;
            return Instance._fovTween = DOTween.To(x => _currentVirtualCamera.m_Lens.FieldOfView = x, Instance._startFov, Instance._startFov * multiplier, duration).SetEase(GetEase(ease));
        }

        #endregion

        [Serializable]
        public class OffsetRotation
        {
            public CinemachinePos cinemachinePos;
            public Vector3 offset;
            public Vector3 rotation;
        }

        [Serializable]
        public class CinemachineInfo
        {
            public CinemachinePos cinemachinePos;
            public CinemachineVirtualCamera cinemachineVirtualCamera;
            public bool ignoreTimeScale;
        }
    }
}