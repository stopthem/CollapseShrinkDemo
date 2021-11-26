using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
using CanTemplate.Extensions;

public class CinemachineManager : Singleton<CinemachineManager>
{
    public enum Offsets : int
    {
        IDLE,
        FOLLOW
    }
    private const float speed = 2.5f;

    [System.Serializable]
    private class OffsetRotation
    {
        public string placeHolderName;
        public Vector3 offset;
        public Vector3 rotation;
    }
    [SerializeField] private OffsetRotation[] offsetsRotations;
    public static Offsets _currentOffset;
    [Header("Shake")]
    [SerializeField] NoiseSettings shakeNoise;
    [SerializeField] private EaseSelection shakeFinishEase;
    [SerializeField] private float shakeFinishDuration = 1f;
    private bool _shaking;
    private static CinemachineVirtualCamera cinemachineVirtualCamera;
    private static CinemachineTransposer transposer;
    private static CinemachineBasicMultiChannelPerlin multiChannelPerlin;

    private void Awake()
    {
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        transposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        multiChannelPerlin = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void Start() => transposer.m_FollowOffset = offsetsRotations[0].offset;

    public static void Shake(float intensity, float duration)
    {
        if (Instance._shaking) return;
        Instance._shaking = true;
        NoiseSettings oldNoise = multiChannelPerlin.m_NoiseProfile;
        float oldAmplitudeGain = multiChannelPerlin.m_AmplitudeGain;
        multiChannelPerlin.m_AmplitudeGain = intensity;
        multiChannelPerlin.m_NoiseProfile = Instance.shakeNoise;
        DOTween.Sequence()
        .AppendInterval(duration)
        .Append(DOTween.To(x => multiChannelPerlin.m_AmplitudeGain = x, intensity, 0, duration))
            .AppendCallback(() => multiChannelPerlin.m_NoiseProfile = oldNoise)
        .Append(DOTween.To(x => multiChannelPerlin.m_AmplitudeGain = x, multiChannelPerlin.m_AmplitudeGain, oldAmplitudeGain, Instance.shakeFinishDuration)
                .SetEase(Instance.shakeFinishEase))
        .AppendCallback(() => Instance._shaking = false);
    }

    public void EmptyFollowTarget(bool emptyLookAt = false)
    {
        cinemachineVirtualCamera.m_Follow = null;
        cinemachineVirtualCamera.m_LookAt = emptyLookAt ? null : cinemachineVirtualCamera.m_LookAt;
    }

    public static Tween MoveOnlyPos(Offsets offset, float duration, Ease ease = Ease.InOutQuad)
    => DOVirtual.Vector3(transposer.m_FollowOffset, Instance.offsetsRotations[(int)offset].offset, duration, x => transposer.m_FollowOffset = x).SetEase(ease)
    .OnComplete(() => _currentOffset = offset);

    public static Tween MoveOnlyRot(Offsets offset, float duration, Ease ease = Ease.InOutQuad)
    => Instance.transform.DORotate(Instance.offsetsRotations[(int)offset].rotation, duration).SetEase(ease)
    .OnComplete(() => _currentOffset = offset);

    public static Sequence Move(Offsets offset, float duration, Ease ease = Ease.InOutQuad)
    {
        return DOTween.Sequence()
         .Append(DOVirtual.Vector3(transposer.m_FollowOffset, Instance.offsetsRotations[(int)offset].offset, duration, x => transposer.m_FollowOffset = x).SetEase(ease))
         .Join(Instance.transform.DORotate(Instance.offsetsRotations[(int)offset].rotation, duration).SetEase(ease))
         .AppendCallback(() => _currentOffset = offset);
    }

    public static void ChangeUpdateMethod(CinemachineBrain.UpdateMethod updateMethod)
    {
        Camera.main.GetComponent<CinemachineBrain>().m_UpdateMethod = updateMethod;
        Camera.main.GetComponent<CinemachineBrain>().m_BlendUpdateMethod = CinemachineBrain.BrainUpdateMethod.FixedUpdate;
    }
}
