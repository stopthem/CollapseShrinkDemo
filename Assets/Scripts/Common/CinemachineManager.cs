using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

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
    [SerializeField] private PresetAnimationCurves shakeFinishCurve = PresetAnimationCurves.INCREASE;
    [SerializeField] private float shakeFinishSpeed = 5f;
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

    private void Start()
    {
        transposer.m_FollowOffset = offsetsRotations[0].offset;
    }

    public static void Shake(float intensity, float duration)
    {
        if (Instance._shaking) return;
        Instance._shaking = true;
        NoiseSettings oldNoise = multiChannelPerlin.m_NoiseProfile;
        float oldAmplitudeGain = multiChannelPerlin.m_AmplitudeGain;
        multiChannelPerlin.m_AmplitudeGain = intensity;
        multiChannelPerlin.m_NoiseProfile = Instance.shakeNoise;
        LerpManager.Wait(duration, () =>
        {
            LerpManager.LerpOverTime(intensity, 0, Instance.shakeFinishSpeed, x => multiChannelPerlin.m_AmplitudeGain = x, curve: Instance.shakeFinishCurve,
            normalAction: () =>
            {
                LerpManager.LerpOverTime(multiChannelPerlin.m_AmplitudeGain, oldAmplitudeGain, 2f, x => multiChannelPerlin.m_AmplitudeGain = x,
                 normalAction: () => Instance._shaking = false);
                multiChannelPerlin.m_NoiseProfile = oldNoise;
            });
        });
    }

    public void EmptyFollowTarget(bool emptyLookAt = false)
    {
        cinemachineVirtualCamera.m_Follow = null;
        cinemachineVirtualCamera.m_LookAt = emptyLookAt ? null : cinemachineVirtualCamera.m_LookAt;
    }


    public static void MoveOnlyPos(Offsets offset, float speed = speed, PresetAnimationCurves curve = PresetAnimationCurves.DECREASING, AnimationCurve overrideCurve = null,
    float timeActionWhen = 0, System.Action timeAction = null, float delayAction = 0, System.Action action = null)
    {
        LerpManager.LerpOverTime(transposer.m_FollowOffset, Instance.offsetsRotations[(int)offset].offset, speed, x => transposer.m_FollowOffset = x, curve,
            overrideCurve, timeActionWhen, timeAction, delayAction, () =>
              {
                  action.Invoke();
                  _currentOffset = offset;
              });
    }

    public static void MoveOnlyRot(Offsets offset, float speed = speed, PresetAnimationCurves curve = PresetAnimationCurves.DECREASING, AnimationCurve overrideCurve = null,
    float timeActionWhen = 0f, System.Action timeAction = null, float delayAction = 0, System.Action action = null)
    {
        LerpManager.LerpOverTime(Instance.transform.rotation, Quaternion.Euler(Instance.offsetsRotations[(int)offset].rotation), speed, x => Instance.transform.rotation = x, curve,
            overrideCurve, timeActionWhen, timeAction, delayAction, () =>
              {
                  action.Invoke();
                  _currentOffset = offset;
              });
    }

    public static void Move(Offsets offset, float speed = speed, bool pos = true, bool rot = true, PresetAnimationCurves curve = PresetAnimationCurves.DECREASING,
     AnimationCurve overrideCurve = null,
       float timeActionWhen = 0f, System.Action timeAction = null, float delayAction = 0, System.Action action = null)
    {
        if (pos)
        {
            LerpManager.LerpOverTime(transposer.m_FollowOffset, Instance.offsetsRotations[(int)offset].offset, speed, x => transposer.m_FollowOffset = x, curve,
                overrideCurve, timeActionWhen, timeAction, delayAction, () =>
              {
                  action.Invoke();
                  _currentOffset = offset;
              });
        }
        if (rot)
        {
            System.Action action1 = pos ? null : action;
            System.Action timeAction1 = pos ? null : timeAction;
            LerpManager.LerpOverTime(Instance.transform.rotation, Quaternion.Euler(Instance.offsetsRotations[(int)offset].rotation), speed, x => Instance.transform.rotation = x, curve,
            overrideCurve, timeActionWhen, timeAction1, delayAction, () =>
              {
                  action.Invoke();
                  _currentOffset = offset;
              });
        }
    }

    public static void ChangeUpdateMethod(CinemachineBrain.UpdateMethod updateMethod)
    {
        Camera.main.GetComponent<CinemachineBrain>().m_UpdateMethod = updateMethod;
        Camera.main.GetComponent<CinemachineBrain>().m_BlendUpdateMethod = CinemachineBrain.BrainUpdateMethod.FixedUpdate;
    }
}
