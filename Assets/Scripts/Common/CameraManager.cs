using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using CanTemplate.Extensions;
using UnityEditor;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    public enum Positions
    {
        Start,
        Fight
    }

    [SerializeField] private float defaultDuration;
    [SerializeField] private EaseSelection defaultEase;
    [SerializeField] private PosInfo[] posInfos;
    [HideInInspector] public bool useOtherCam;
    [HideInInspector] public Camera otherCamera;
    public static Positions currentPos;

    private void Awake()
    {
        Instance = this;
    }

    private void Start() => otherCamera = otherCamera ? otherCamera : Camera.main;

    ///<param name ="duration">Duration of the tween. If 0, defaultDuration of this script will be used.</param>
    public static Tween Move(Transform to, float duration = 0, EaseSelection ease = null, bool local = false)
    {
        duration = ReturnDuration(duration);
        ease = ReturnEase(ease);

        return local
            ? Instance.transform.DoLocalMoveRotate(to, duration).SetEase(ease)
            : Instance.transform.DoMoveRotate(to, duration).SetEase(ease);
    }

    ///<param name ="duration">Duration of the tween. If 0, defaultDuration of this script will be used.</param>
    public static Tween Move(Positions position, float duration = 0, EaseSelection ease = null, bool local = false)
    {
        duration = ReturnDuration(duration);
        ease = ReturnEase(ease);

        var tween = local
            ? Instance.transform.DoLocalMoveRotate(GetPosInfo(position), duration).SetEase(ease)
            : Instance.transform.DoMoveRotate(GetPosInfo(position), duration).SetEase(ease);
        tween.OnComplete(() => currentPos = position);
        return tween;
    }

    ///<param name ="duration">Duration of the tween. If 0, defaultDuration of this script will be used.</param>
    public static Tween MoveOnlyRot(Positions position, float duration = 0, EaseSelection ease = null, bool local = false)
    {
        duration = ReturnDuration(duration);
        ease = ReturnEase(ease);

        var tween = local
            ? Instance.transform.DOLocalRotate(GetPosInfoRot(position, true), duration).SetEase(ease)
            : Instance.transform.DORotate(GetPosInfoRot(position), duration).SetEase(ease);

        tween.OnComplete(() => currentPos = position);

        return tween;
    }

    ///<param name ="duration">Duration of the tween. If 0, defaultDuration of this script will be used.</param>
    public static Tween MoveOnlyRot(Vector3 toRot, float duration = 0, EaseSelection ease = null, bool local = false)
    {
        duration = ReturnDuration(duration);
        ease = ReturnEase(ease);

        var tween = local
            ? Instance.transform.DOLocalRotate(toRot, duration).SetEase(ease)
            : Instance.transform.DORotate(toRot, duration).SetEase(ease);

        return tween;
    }

    ///<param name ="duration">Duration of the tween. If 0, defaultDuration of this script will be used.</param>
    public static Tween MoveOnlyPos(Positions position, float duration = 0, EaseSelection ease = null, bool local = false)
    {
        duration = ReturnDuration(duration);
        ease = ReturnEase(ease);

        var tween = local
            ? Instance.transform.DOLocalMove(GetPosInfoPos(position, true), duration).SetEase(ease)
            : Instance.transform.DOMove(GetPosInfoPos(position), duration).SetEase(ease);

        tween.OnComplete(() => currentPos = position);

        return tween;
    }

    ///<param name ="duration">Duration of the tween. If 0, defaultDuration of this script will be used.</param>
    public static Tween MoveOnlyPos(Vector3 toPos, float duration = 0, EaseSelection ease = null, bool local = false)
    {
        duration = ReturnDuration(duration);
        ease = ReturnEase(ease);

        var tween = local
            ? Instance.transform.DOLocalMove(toPos, duration).SetEase(ease)
            : Instance.transform.DOMove(toPos, duration).SetEase(ease);

        return tween;
    }

    private static Vector3 GetPosInfoPos(Positions positions, bool local = false)
    {
        var foundTransform = Instance.posInfos.First(x => x.positions == positions).posTransform;
        return local ? foundTransform.localPosition : foundTransform.position;
    }

    private static Vector3 GetPosInfoRot(Positions positions, bool local = false)
    {
        var foundTransform = Instance.posInfos.First(x => x.positions == positions).posTransform;
        return local ? foundTransform.localRotation.ClampEuler() : foundTransform.rotation.ClampEuler();
    }

    private static Transform GetPosInfo(Positions positions) => Instance.posInfos.First(x => x.positions == positions).posTransform;

    private static float ReturnDuration(float duration) => duration == 0 ? Instance.defaultDuration : duration;
    private static EaseSelection ReturnEase(EaseSelection ease) => ease ?? Instance.defaultEase;

    [System.Serializable]
    private class PosInfo
    {
        public Positions positions;
        public Transform posTransform;
    }
}

#if UNITY_EDITOR
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
#endif