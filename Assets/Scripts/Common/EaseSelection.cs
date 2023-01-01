using System;
using UnityEngine;
using DG.Tweening;
using UnityEditor;
using System.Globalization;

namespace CanTemplate.Extensions
{
    [Serializable]
    public class EaseSelection
    {
        [HideInInspector] public EaseOptions easeOption;
        [HideInInspector] public Ease ease;
        [HideInInspector] public AnimationCurve animationCurve;

        public EaseSelection(Ease ease)
        {
            easeOption = EaseOptions.DOTweenEases;
            this.ease = ease;
        }

        public EaseSelection(AnimationCurve curve)
        {
            easeOption = EaseOptions.AnimationCurve;
            animationCurve = curve;
        }

        public object GetEase()
        {
            switch (easeOption)
            {
                case EaseOptions.DOTweenEases:
                    return ease;
                default: return animationCurve;
            }
        }
    }

    public enum EaseOptions
    {
        DOTweenEases,
        AnimationCurve
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(EaseSelection))]
    public class EaseSelectionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            if (EditorGUI.indentLevel != 0)
            {
                EditorGUI.indentLevel--;
            }

            var optionRect = new Rect(position.x + position.width / 2, position.y, position.width - position.width / 2, position.height);

            var easeOption = property.FindPropertyRelative("easeOption");
            var selectionFromInspector = easeOption.intValue;
            var easeOptionNames = Enum.GetNames(typeof(EaseOptions));
            for (int i = 0; i < easeOptionNames.Length; i++)
            {
                TextInfo textInfo = CultureInfo.InvariantCulture.TextInfo;
                easeOptionNames[i] = textInfo.ToTitleCase(easeOptionNames[i].ToLowercaseNamingConvention(true));
            }

            var actualSelected = EditorGUI.Popup(new Rect(position.x, position.y, position.width / 2, position.height), selectionFromInspector, easeOptionNames);
            easeOption.intValue = actualSelected;

            var propertyName = actualSelected == 0 ? "ease" : "animationCurve";
            EditorGUI.PropertyField(optionRect, property.FindPropertyRelative(propertyName), GUIContent.none);

            if (EditorGUI.indentLevel != 0)
            {
                EditorGUI.indentLevel++;
            }


            EditorGUI.EndProperty();
        }
    }
#endif
}