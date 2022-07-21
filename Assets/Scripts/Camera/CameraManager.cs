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
    [SerializeField] private Ease defaultEase = Ease.OutSine;
    [SerializeField] private PosInfo[] posInfos;
    [HideInInspector] public bool useOtherCam;
    [HideInInspector] public Camera otherCamera;
    public static Positions CurrentPos { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start() => otherCamera = otherCamera ? otherCamera : Camera.main;

    #region Move Position And Rotation

    ///<param name="ease">If null, this scripts defaultEase will be used.</param>
    ///<param name ="duration">Duration of the tween. If null, defaultDuration of this script will be used.</param>
    public static Tween Move(Transform to, float? duration = null, Ease? ease = null, bool local = false)
    {
        var actualDuration = GetDuration(duration);

        ease = GetEase(ease);

        return local
            ? Instance.transform.DoLocalMoveRotate(to, actualDuration).SetEase(ease!.Value)
            : Instance.transform.DoMoveRotate(to, actualDuration).SetEase(ease!.Value);
    }

    ///<param name="ease">If null, this scripts defaultEase will be used.</param>
    ///<param name ="duration">Duration of the tween. If null, defaultDuration of this script will be used.</param>
    public static Tween Move(Positions position, float? duration = null, Ease? ease = null, bool local = false)
    {
        var actualDuration = GetDuration(duration);

        ease = GetEase(ease);

        var tween = local
            ? Instance.transform.DoLocalMoveRotate(GetPosInfo(position), actualDuration).SetEase(ease!.Value)
            : Instance.transform.DoMoveRotate(GetPosInfo(position), actualDuration).SetEase(ease!.Value);
        tween.OnComplete(() => CurrentPos = position);
        return tween;
    }

    #endregion

    #region Move Only Rotation
    ///<param name="ease">If null, this scripts defaultEase will be used.</param>
    ///<param name ="duration">Duration of the tween. If null, defaultDuration of this script will be used.</param>
    public static Tween MoveOnlyRot(Positions position, float? duration = null, Ease? ease = null, bool local = false)
    {
        var actualDuration = GetDuration(duration);

        ease = GetEase(ease);

        var tween = local
            ? Instance.transform.DOLocalRotate(GetPosInfoRot(position, true), actualDuration).SetEase(ease!.Value)
            : Instance.transform.DORotate(GetPosInfoRot(position), actualDuration).SetEase(ease!.Value);

        tween.OnComplete(() => CurrentPos = position);

        return tween;
    }

    ///<param name ="duration">Duration of the tween. If null, defaultDuration of this script will be used.</param>
    public static Tween MoveOnlyRot(Vector3 toRot, float? duration = null, Ease? ease = null, bool local = false)
    {
        var actualDuration = GetDuration(duration);

        ease = GetEase(ease);

        var tween = local
            ? Instance.transform.DOLocalRotate(toRot, actualDuration).SetEase(ease!.Value)
            : Instance.transform.DORotate(toRot, actualDuration).SetEase(ease!.Value);

        return tween;
    }

    #endregion

    #region Move Only Position
    
    ///<param name="ease">If null, this scripts defaultEase will be used.</param>
    ///<param name ="duration">Duration of the tween. If null, defaultDuration of this script will be used.</param>
    public static Tween MoveOnlyPos(Positions position, float? duration = null, Ease? ease = null, bool local = false)
    {
        var actualDuration = GetDuration(duration);

        ease = GetEase(ease);

        var tween = local
            ? Instance.transform.DOLocalMove(GetPosInfoPos(position, true), actualDuration).SetEase(ease!.Value)
            : Instance.transform.DOMove(GetPosInfoPos(position), actualDuration).SetEase(ease!.Value);

        tween.OnComplete(() => CurrentPos = position);

        return tween;
    }
    
    ///<param name="ease">If null, this scripts defaultEase will be used.</param>
    ///<param name ="duration">Duration of the tween. If null, defaultDuration of this script will be used.</param>
    public static Tween MoveOnlyPos(Vector3 toPos, float? duration = null, Ease? ease = null, bool local = false)
    {
        var actualDuration = GetDuration(duration);
        ease = GetEase(ease);

        var tween = local
            ? Instance.transform.DOLocalMove(toPos, actualDuration).SetEase(ease!.Value)
            : Instance.transform.DOMove(toPos, actualDuration).SetEase(ease!.Value);

        return tween;
    }

    #endregion

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

    private static float GetDuration(float? duration) => duration ?? Instance.defaultDuration;
    private static Ease GetEase(Ease? ease) => ease ?? Instance.defaultEase;

    [System.Serializable]
    private class PosInfo
    {
        public Positions positions;
        public Transform posTransform;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(CameraManager))]
public class CameraManagerEditor : Editor
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