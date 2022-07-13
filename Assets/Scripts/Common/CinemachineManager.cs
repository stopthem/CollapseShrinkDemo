using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
using CanTemplate.Extensions;

public class CinemachineManager : MonoBehaviour
{
    public static CinemachineManager Instance;

    public enum Offsets : int
    {
        BeforePunch,
        AfterPunch
    }

    private const float _defaultDuration = .5f;

    [System.Serializable]
    private class OffsetRotation
    {
        public Offsets offsets;
        public Vector3 offset;
        public Vector3 rotation;
    }

    [SerializeField] private OffsetRotation[] offsetsRotations;
    public static Offsets currentOffset;
    [Header("Shake")] [SerializeField] private NoiseSettings shakeNoise;
    [SerializeField] private EaseSelection shakeFinishEase;
    [SerializeField] private float shakeFinishDuration = 1f;
    private bool _shaking;
    private static CinemachineVirtualCamera _cinemachineVirtualCamera;
    private static CinemachineTransposer _transposer;
    private static CinemachineBasicMultiChannelPerlin _multiChannelPerlin;

    private void Awake()
    {
        Instance = this;
        _cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        _transposer = _cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        _multiChannelPerlin = _cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void Start() => _transposer.m_FollowOffset = offsetsRotations[0].offset;

    public static void Shake(float intensity, float duration)
    {
        if (Instance._shaking) return;
        Instance._shaking = true;
        NoiseSettings oldNoise = _multiChannelPerlin.m_NoiseProfile;
        float oldAmplitudeGain = _multiChannelPerlin.m_AmplitudeGain;
        _multiChannelPerlin.m_AmplitudeGain = intensity;
        _multiChannelPerlin.m_NoiseProfile = Instance.shakeNoise;
        DOTween.Sequence()
            .AppendInterval(duration)
            .Append(DOTween.To(x => _multiChannelPerlin.m_AmplitudeGain = x, intensity, 0, duration))
            .AppendCallback(() => _multiChannelPerlin.m_NoiseProfile = oldNoise)
            .Append(DOTween.To(x => _multiChannelPerlin.m_AmplitudeGain = x, _multiChannelPerlin.m_AmplitudeGain, oldAmplitudeGain, Instance.shakeFinishDuration)
                .SetEase(Instance.shakeFinishEase))
            .AppendCallback(() => Instance._shaking = false);
    }

    public void EmptyFollowTarget(bool emptyLookAt = false)
    {
        _cinemachineVirtualCamera.m_Follow = null;
        _cinemachineVirtualCamera.m_LookAt = emptyLookAt ? null : _cinemachineVirtualCamera.m_LookAt;
    }

    public static Tween MoveOnlyPos(Offsets offset, float duration = _defaultDuration, Ease ease = Ease.OutSine)
        => DOVirtual.Vector3(_transposer.m_FollowOffset, Instance.offsetsRotations[(int)offset].offset, duration, x => _transposer.m_FollowOffset = x).SetEase(ease)
            .OnComplete(() => currentOffset = offset);

    public static Tween MoveOnlyRot(Offsets offset, float duration = _defaultDuration, Ease ease = Ease.OutSine)
        => Instance.transform.DORotate(Instance.offsetsRotations[(int)offset].rotation, duration).SetEase(ease)
            .OnComplete(() => currentOffset = offset);

    public static Sequence Move(Offsets offset, float duration = _defaultDuration, Ease ease = Ease.OutSine)
    {
        return DOTween.Sequence()
            .Append(DOVirtual.Vector3(_transposer.m_FollowOffset, Instance.offsetsRotations[(int)offset].offset, duration, x => _transposer.m_FollowOffset = x).SetEase(ease))
            .Join(Instance.transform.DORotate(Instance.offsetsRotations[(int)offset].rotation, duration).SetEase(ease))
            .AppendCallback(() => currentOffset = offset);
    }

    public static void ChangeUpdateMethod(CinemachineBrain.UpdateMethod updateMethod)
    {
        Instance.GetComponent<CinemachineBrain>().m_UpdateMethod = updateMethod;
        Instance.GetComponent<CinemachineBrain>().m_BlendUpdateMethod = CinemachineBrain.BrainUpdateMethod.FixedUpdate;
    }
}