using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineManager : MonoBehaviour
{
    public enum Positions : int
    {

    }

    private static CinemachineManager instance;
    [SerializeField] private Camera otherCamera;
    [SerializeField] private Vector3[] positionsInOrder;
    [SerializeField] private Vector3[] rotationsInOrder;

    private CinemachineVirtualCamera cinemachineVirtualCamera;
    private CinemachineTransposer transposer;
    private CinemachineBasicMultiChannelPerlin multiChannelPerlin;

    private float m_shakeStartIntensity, m_shakeDuration, m_shakeTotal;

    private void Awake()
    {
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        transposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        multiChannelPerlin = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        instance = this;
    }

    private void Start()
    {
        //eğer başka bir kamera kullanılmayacaksa main camera kullanılacak.
        if (otherCamera == null)
        {
            otherCamera = Camera.main;
        }
    }

    private void Update()
    {
        if (m_shakeDuration > 0f)
        {
            m_shakeDuration -= Time.deltaTime;
            if (m_shakeDuration <= 0f)
            {
                multiChannelPerlin.m_AmplitudeGain = Mathf.Lerp(m_shakeStartIntensity, 0f, 1 - (m_shakeDuration / m_shakeTotal));
            }
        }
    }

    public static void Shake(float intensity, float duration)
    {
        instance.m_shakeDuration = duration;
        instance.m_shakeStartIntensity = intensity;
        instance.multiChannelPerlin.m_AmplitudeGain = intensity;
        instance.m_shakeTotal = duration;
    }

    public static void MoveOnlyPos(Positions position, float speed, AnimationCurve overrideCurve = null,
     LerpManager.Curves curve = LerpManager.Curves.DECREASING, float delayAction = 0, System.Action action = null)
    {
        LerpManager.LerpOverTime(instance.transposer.m_FollowOffset, instance.positionsInOrder[(int)position], speed, x => instance.transposer.m_FollowOffset = x, overrideCurve,
            curve, action, delayAction);
    }

    public static void MoveOnlyRot(Positions position, float speed, AnimationCurve overrideCurve = null,
     LerpManager.Curves curve = LerpManager.Curves.DECREASING, float delayAction = 0, System.Action action = null)
    {
        LerpManager.LerpOverTime(instance.transform.rotation, Quaternion.Euler(instance.rotationsInOrder[(int)position]), speed, x => instance.transform.rotation = x, overrideCurve,
            curve, action, delayAction);
    }
}
