using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditor;
using System.Globalization;
using System.Threading;
using CanTemplate.Extensions;

[System.Serializable]
public class EaseSelection
{
    [HideInInspector] public EaseOptions easeOption;
    [HideInInspector] public Ease ease;
    [HideInInspector] public PresetAnimationCurves presetAnimationCurve;
    [HideInInspector] public AnimationCurve animationCurve;

    public EaseSelection(Ease ease)
    {
        easeOption = EaseOptions.DOTweenEases;
        this.ease = ease;
    }

    public EaseSelection(PresetAnimationCurves presetAnimationCurve)
    {
        easeOption = EaseOptions.LerpManagerCurves;
        this.presetAnimationCurve = presetAnimationCurve;
    }

    public EaseSelection(AnimationCurve curve)
    {
        easeOption = EaseOptions.CustomAnimationCurve;
        animationCurve = curve;
    }

    public object GetEase()
    {
        switch (easeOption)
        {
            case EaseOptions.DOTweenEases:
                return ease;
            case EaseOptions.LerpManagerCurves:
                return LerpManager.PresetToAnimationCurve(presetAnimationCurve);
            default: return animationCurve;
        }
    }
}

public enum EaseOptions
{
    DOTweenEases,
    LerpManagerCurves,
    CustomAnimationCurve
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(EaseSelection))]
public class EaseSelectionDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Keyboard), label);

        int originalIndent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        EditorGUI.BeginProperty(position, label, property);

        var optionRect = new Rect(position.x + position.width / 2, position.y, position.width - position.width / 2, position.height);

        SerializedProperty easeOption = property.FindPropertyRelative("easeOption");
        int actualSelected = 1;
        int selectionFromInspector = easeOption.intValue;
        string[] easeOptionNames = System.Enum.GetNames(typeof(EaseOptions));
        for (int i = 0; i < easeOptionNames.Length; i++)
        {
            TextInfo textInfo = CultureInfo.InvariantCulture.TextInfo;
            easeOptionNames[i] = textInfo.ToTitleCase(easeOptionNames[i].ToLowercaseNamingConvention(true));
        }

        actualSelected = EditorGUI.Popup(new Rect(position.x, position.y, position.width / 2, position.height), selectionFromInspector, easeOptionNames);
        easeOption.intValue = actualSelected;

        string propertyName = actualSelected == 0 ? "ease" : actualSelected == 1 ? "presetAnimationCurve" : "animationCurve";
        EditorGUI.PropertyField(optionRect, property.FindPropertyRelative(propertyName), GUIContent.none);

        EditorGUI.EndProperty();

        EditorGUI.indentLevel = originalIndent;
    }
}
#endif