using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    public enum Positions
    {
        PLAYER,
        FINISH
    }
    private const float speed = 2.25f;

    [SerializeField] private Camera otherCamera;
    [SerializeField] private Transform positionsHolder;
    [HideInInspector] public Positions _currentPos;
    private Transform[] positionsInOrder;

    private void Awake()
    {
        positionsInOrder = positionsHolder.GetComponentsInChildren<Transform>().Where(x => x.transform != positionsHolder).ToArray();
    }

    private void Start()
    {
        if (otherCamera == null)
        {
            otherCamera = Camera.main;
        }
    }

    public static void Move(Positions position, float speed = speed, bool pos = true, bool rot = true, PresetAnimationCurves curve = PresetAnimationCurves.DECREASING,
     AnimationCurve overrideCurve = null, float timeActionWhen = 0, System.Action timeAction = null, float delayAction = 0, System.Action action = null)
    {
        if (pos)
        {
            LerpManager.LerpOverTime(Instance.transform.position, Instance.positionsInOrder[(int)position].position, speed, x => Instance.transform.position = x, curve,
            overrideCurve, timeActionWhen, timeAction, delayAction, () =>
              {
                  action?.Invoke();
                  Instance._currentPos = position;
              });
        }
         if (rot)
        {
            System.Action action1 = pos ? null : action;
            System.Action timeAction1 = pos ? null : timeAction;
            LerpManager.LerpOverTime(Instance.transform.rotation, Instance.positionsInOrder[(int)position].rotation, speed, x => Instance.transform.rotation = x, curve,
            overrideCurve, timeActionWhen, timeAction1, delayAction, () =>
              {
                  action1?.Invoke();
                  Instance._currentPos = position;
              });
        }
    }

    public static void MoveOnlyPos(Vector3 targetPosition, float speed = speed, PresetAnimationCurves curve = PresetAnimationCurves.DECREASING,
     AnimationCurve overrideCurve = null, float timeActionWhen = 0, System.Action timeAction = null, float delayAction = 0, System.Action action = null)
    {
        LerpManager.LerpOverTime(Instance.transform.position, targetPosition, speed, x => Instance.transform.position = x, curve,
           overrideCurve, timeActionWhen, timeAction, delayAction, action);
    }

    public static void MoveOnlyRot(Quaternion targetRot, float speed = speed, PresetAnimationCurves curve = PresetAnimationCurves.DECREASING,
     AnimationCurve overrideCurve = null, float timeActionWhen = 0, System.Action timeAction = null, float delayAction = 0, System.Action action = null)
    {
        LerpManager.LerpOverTime(Instance.transform.rotation, targetRot, speed, x => Instance.transform.rotation = x, curve,
            overrideCurve, timeActionWhen, timeAction, delayAction, action);
    }
}
