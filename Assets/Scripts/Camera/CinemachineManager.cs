using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CanTemplate;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
using CanTemplate.Extensions;
using CanTemplate.Utilities;
using DG.DeInspektor.Attributes;
using JetBrains.Annotations;

[RequireComponent(typeof(Camera))]
public class CinemachineManager : MonoBehaviour
{
    public static CinemachineManager instance;

    public static Camera MainCam;

    public enum CinemachinePos
    {
        Player,
        Deck,
        Fight
    }

    [SerializeField, DeBeginGroup] private CinemachinePos startingVirtualCamera;
    [SerializeField, DeEndGroup] private CinemachineInfo[] cinemachineInfos;
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

    [Header("Default Tween Params"), Space(5), SerializeField]
    private TweenOptions defaultTweenOptions;

    [Header("Shake"), Space(5), SerializeField]
    private NoiseSettings shakeNoise;

    [SerializeField] private Ease shakeFinishEase = Ease.OutSine;
    private Sequence _shakeSeq;

    private static CinemachineTransposer _transposer;
    private static CinemachineBasicMultiChannelPerlin _multiChannelPerlin;
    private static CinemachineBrain _cinemachineBrain;

    public float StartFov { get; private set; }
    private Tween _fovTween;

    private void Awake()
    {
        instance = this;
        MainCam = Camera.main;

        _cinemachineBrain = GetComponent<CinemachineBrain>();

        foreach (var vc in cinemachineInfos.Select(x => x.cinemachineVirtualCamera))
            vc.enabled = false;

        ChangeVirtualCamera(startingVirtualCamera, blend: false);
        if (_currentVirtualCamera) UpdateCinemachineVariables();
    }

    private static void UpdateCinemachineVariables()
    {
        _transposer = _currentVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        _multiChannelPerlin = _currentVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void Start()
    {
        if (_currentVirtualCamera) StartFov = _currentVirtualCamera.m_Lens.FieldOfView;

        if (offsetsRotations.ElementAtOrDefault(0) != null)
        {
            _transposer.m_FollowOffset = offsetsRotations[0].offset;
        }
    }

    public static void Shake(ShakeInfo shakeInfo, bool overridePreviousShake = true)
    {
        if (instance._shakeSeq.IsActiveNPlaying())
        {
            if (overridePreviousShake) instance._shakeSeq.Complete();
            else return;
        }

        _multiChannelPerlin = _currentVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        if (!_multiChannelPerlin)
        {
            Debug.Log(_currentVirtualCamera + " has no multi channel perlin!");
        }

        var oldNoise = _multiChannelPerlin.m_NoiseProfile;
        var oldAmplitudeGain = _multiChannelPerlin.m_AmplitudeGain;

        _multiChannelPerlin.m_AmplitudeGain = shakeInfo.intensity;
        // _multiChannelPerlin.m_NoiseProfile = instance.shakeNoise;

        instance._shakeSeq = DOTween.Sequence()
            .Append(DOTween.To(x => _multiChannelPerlin.m_AmplitudeGain = x, shakeInfo.intensity, 0, shakeInfo.duration))
            // .AppendCallback(() => _multiChannelPerlin.m_NoiseProfile = oldNoise)
            .Append(DOTween.To(x => _multiChannelPerlin.m_AmplitudeGain = x, _multiChannelPerlin.m_AmplitudeGain, oldAmplitudeGain, shakeInfo.smoothFinishDuration)
                .SetEase(instance.shakeFinishEase))
            .SetUpdate(true);
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

    public static void SetOffsetX(float toX) => _transposer.m_FollowOffset = _transposer.m_FollowOffset.WithX(toX);

    public static Tween MoveOnlyPos(CinemachinePos cinemachinePos, float? duration = null, Ease? ease = null)
    {
        _currentVirtualCamera.DOKill();

        return DOVirtual.Vector3(_transposer.m_FollowOffset, instance.offsetsRotations[(int)cinemachinePos].offset, GetDuration(duration), x => _transposer.m_FollowOffset = x).SetEase(GetEase(ease))
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
        => instance.transform.DORotate(instance.offsetsRotations[(int)cinemachinePos].rotation, GetDuration(duration)).SetEase(GetEase(ease))
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
        => instance.transform.DORotate(toRot, GetDuration(duration)).SetEase(GetEase(ease))
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
            .Append(DOVirtual.Vector3(_transposer.m_FollowOffset, instance.offsetsRotations[(int)cinemachinePo].offset, GetDuration(duration), x => _transposer.m_FollowOffset = x).SetEase(GetEase(ease)))
            .Join(instance.transform.DORotate(instance.offsetsRotations[(int)cinemachinePo].rotation, GetDuration(duration)).SetEase(GetEase(ease)))
            .SetTarget(_currentVirtualCamera)
            .AppendCallback(() => _currentCinemachinePos = cinemachinePo);
    }

    #endregion

    private static Ease GetEase(Ease? ease) => ease ?? instance.defaultTweenOptions.ease;
    private static float GetDuration(float? duration) => duration ?? instance.defaultTweenOptions.duration;

    public static void ChangeUpdateMethod(CinemachineBrain.UpdateMethod updateMethod)
    {
        _cinemachineBrain.m_UpdateMethod = updateMethod;
        _cinemachineBrain.m_BlendUpdateMethod = CinemachineBrain.BrainUpdateMethod.FixedUpdate;
    }

    public static OffsetRotation GetOffsetRotation(CinemachinePos cinemachinePos) => instance.offsetsRotations.First(x => x.cinemachinePos == cinemachinePos);
    public static CinemachineInfo GetCinemachineInfo(CinemachinePos cinemachinePos) => instance.cinemachineInfos.First(x => x.cinemachinePos == cinemachinePos);

    public static void ChangeVirtualCamera(CinemachinePos toPos, CinemachineBlendDefinition? cinemachineBlendDefinition = null, bool blend = true, TweenCallback onBlendFinished = null)
    {
        instance.StopAllCoroutines();

        var newVcInfo = instance.cinemachineInfos.FirstOrDefault(x => x.cinemachinePos == toPos);

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
            () => instance.StartCoroutine(BlendWaitCoroutine(() =>
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
        instance._fovTween.Kill();
        _currentVirtualCamera.m_Lens.FieldOfView = instance.StartFov;
        return instance._fovTween = DOTween.To(x => _currentVirtualCamera.m_Lens.FieldOfView = x, instance.StartFov, value, duration).SetEase(GetEase(ease));
    }

    public static void DoFov(float value, bool multiplier = false)
    {
        instance._fovTween.Kill();
        _currentVirtualCamera.m_Lens.FieldOfView = multiplier ? instance.StartFov * value : value;
    }

    public static Tween DoMultiplierFov(float multiplier, float duration, Ease? ease = null)
    {
        instance._fovTween.Kill();
        _currentVirtualCamera.m_Lens.FieldOfView = instance.StartFov;
        return instance._fovTween = DOTween.To(x => _currentVirtualCamera.m_Lens.FieldOfView = x, instance.StartFov, instance.StartFov * multiplier, duration).SetEase(GetEase(ease));
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
        public bool ignoreTimeScale = false;
    }
}