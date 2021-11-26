using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using CanTemplate.Extensions;
using UnityEditor;

public class CameraManager : Singleton<CameraManager>
{
    public enum Positions
    {
        Start,
        Teeth,
        Hair
    }

    [SerializeField] private float defaultDuration;
    [SerializeField] private EaseSelection defaultEase;

    [HideInInspector] public bool useOtherCam;
    [HideInInspector] public Camera otherCamera;
    [SerializeField] private Transform positionsHolder;
    public static Positions currentPos;
    private Transform[] positionsInOrder;

    private void Awake()
    => positionsInOrder = positionsHolder.GetComponentsInChildren<Transform>().Where(x => x.transform != positionsHolder).ToArray();

    private void Start() => otherCamera = otherCamera ? otherCamera : Camera.main;

    ///<param name ="duration">Duration of the tween. If 0, defaultDuration of this script will be used.</param>
    public static Tween Move(Transform to, float duration = 0, EaseSelection ease = null, bool local = false)
    {
        duration = ReturnDuration(duration);
        ease = ReturnEase(ease);

        return local ? Instance.transform.DoLocalMoveRotate(to, duration).SetEase(ease)
        : Instance.transform.DoMoveRotate(to, duration).SetEase(ease);
    }

    ///<param name ="duration">Duration of the tween. If 0, defaultDuration of this script will be used.</param>
    public static Tween Move(Positions position, float duration = 0, EaseSelection ease = null, bool local = false)
    {
        duration = ReturnDuration(duration);
        ease = ReturnEase(ease);

        var tween = local ? Instance.transform.DoLocalMoveRotate(Instance.positionsInOrder[(int)position], duration).SetEase(ease)
        : Instance.transform.DoMoveRotate(Instance.positionsInOrder[(int)position], duration).SetEase(ease);
        tween.OnComplete(() => currentPos = position);
        return tween;
    }

    ///<param name ="duration">Duration of the tween. If 0, defaultDuration of this script will be used.</param>
    public static Tween MoveOnlyRot(Positions position, float duration = 0, EaseSelection ease = null, bool local = false)
    {
        duration = ReturnDuration(duration);
        ease = ReturnEase(ease);

        var tween = local ? Instance.transform.DOLocalRotate(Instance.positionsInOrder[(int)position].localRotation.ClampEuler(), duration).SetEase(ease)
        : Instance.transform.DORotate(Instance.positionsInOrder[(int)position].rotation.ClampEuler(), duration).SetEase(ease);

        tween.OnComplete(() => currentPos = position);

        return tween;
    }

    ///<param name ="duration">Duration of the tween. If 0, defaultDuration of this script will be used.</param>
    public static Tween MoveOnlyRot(Vector3 toRot, float duration = 0, EaseSelection ease = null, bool local = false)
    {
        duration = ReturnDuration(duration);
        ease = ReturnEase(ease);

        var tween = local ? Instance.transform.DOLocalRotate(toRot, duration).SetEase(ease)
        : Instance.transform.DORotate(toRot, duration).SetEase(ease);

        return tween;
    }

    ///<param name ="duration">Duration of the tween. If 0, defaultDuration of this script will be used.</param>
    public static Tween MoveOnlyPos(Positions position, float duration = 0, EaseSelection ease = null, bool local = false)
    {
        duration = ReturnDuration(duration);
        ease = ReturnEase(ease);

        var tween = local ? Instance.transform.DOLocalMove(Instance.positionsInOrder[(int)position].localPosition, duration).SetEase(ease)
        : Instance.transform.DOMove(Instance.positionsInOrder[(int)position].position, duration).SetEase(ease);

        tween.OnComplete(() => currentPos = position);

        return tween;
    }

    ///<param name ="duration">Duration of the tween. If 0, defaultDuration of this script will be used.</param>
    public static Tween MoveOnlyPos(Vector3 toPos, float duration = 0, EaseSelection ease = null, bool local = false)
    {
        duration = ReturnDuration(duration);
        ease = ReturnEase(ease);

        var tween = local ? Instance.transform.DOLocalMove(toPos, duration).SetEase(ease)
        : Instance.transform.DOMove(toPos, duration).SetEase(ease);

        return tween;
    }

    private static float ReturnDuration(float duration) => duration == 0 ? Instance.defaultDuration : duration;
    private static EaseSelection ReturnEase(EaseSelection ease) => ease == null ? Instance.defaultEase : ease;
}

[CustomEditor(typeof(CameraManager))]
public class CameraManager_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        CameraManager script = target as CameraManager;

        EditorGUILayout.Space(5);

        EditorGUILayout.BeginHorizontal();

        script.useOtherCam = EditorGUILayout.Toggle(new GUIContent("Use Other Cam", "If not, Camera.Main will be used."), script.useOtherCam, GUILayout.ExpandWidth(false));
        if (script.useOtherCam) script.otherCamera = (Camera)EditorGUILayout.ObjectField(script.otherCamera, typeof(Camera), true);

        EditorGUILayout.EndHorizontal();
    }
}
