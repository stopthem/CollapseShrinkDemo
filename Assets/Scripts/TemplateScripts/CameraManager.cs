using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public enum Positions : int
    {
        PLAYER = 0,
        FINISH = 1
    }
    private const float speed = 2.25f;

    public static CameraManager instance;
    [SerializeField] private Camera otherCamera;
    [SerializeField] private Transform positionsHolder;
    private Transform[] positionsInOrder;

    private void Awake()
    {
        instance = this;
        positionsInOrder = positionsHolder.GetComponentsInChildren<Transform>().Where(x => x.transform != positionsHolder).ToArray();
    }

    private void Start()
    {
        //eğer başka bir kamera kullanılmayacaksa main camera kullanılacak.
        if (otherCamera == null)
        {
            otherCamera = Camera.main;
        }
    }

    public static void Move(Positions position, float speed = speed, bool pos = true, bool rot = true, AnimationCurve overrideCurve = null,
     LerpManager.Curves curve = LerpManager.Curves.DECREASING, float delayAction = 0, System.Action action = null)
    {
        if (pos)
        {
            LerpManager.LerpOverTime(instance.transform.position, instance.positionsInOrder[(int)position].position, speed, x => instance.transform.position = x, overrideCurve,
            curve, action, delayAction);
        }
        if (rot)
        {
            System.Action action1 = pos ? null : action;
            LerpManager.LerpOverTime(instance.transform.rotation, instance.positionsInOrder[(int)position].rotation, speed, x => instance.transform.rotation = x, overrideCurve,
            curve, action1, 0, null, delayAction);
        }
    }

    public static void MoveOnlyPos(Vector3 targetPosition, float speed = speed, AnimationCurve overrideCurve = null,
     LerpManager.Curves curve = LerpManager.Curves.DECREASING, float delayAction = 0, System.Action action = null)
    {
        LerpManager.LerpOverTime(instance.transform.position, targetPosition, speed, x => instance.transform.position = x, overrideCurve,
            curve, action, delayAction);
    }

    public static void MoveOnlyRot(Quaternion targetRot, float speed = speed, AnimationCurve overrideCurve = null,
     LerpManager.Curves curve = LerpManager.Curves.DECREASING, float delayAction = 0, System.Action action = null)
    {
        LerpManager.LerpOverTime(instance.transform.rotation, targetRot, speed, x => instance.transform.rotation = x, overrideCurve,
            curve, action, delayAction);
    }
}
