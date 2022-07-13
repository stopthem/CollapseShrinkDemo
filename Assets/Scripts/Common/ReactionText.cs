using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class ReactionText : Singleton<ReactionText>
{
    public enum Directions
    {
        TopToBottom,
        RightToLeft,
        LeftToRight
    }

    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Vector2 topToBottomStartPos = new Vector2(0, 1042);
    [SerializeField] private Vector2 topToBottomFinishPos = new Vector2(0, 480);
    [SerializeField] private Vector2 rightToLeftStartPos = new Vector2(840, 500);
    [SerializeField] private Vector2 rightToLeftFinishPos = new Vector2(-860, 500);
    [SerializeField] private Vector2 leftToRightStartPos = new Vector2(-860, 500);
    [SerializeField] private Vector2 leftToRightFinishPos = new Vector2(840, 500);
    private RectTransform rectTransform;

    [SerializeField] private float topToBottomSpeed = 2.5f, xSpeed = 1;

    [SerializeField] private Ease topToBottomEase = Ease.OutBack;
    [SerializeField] private AnimationCurve xCurve;

    private const float _startFontSize = 150;

    private bool flag = false;
    private bool nextTextInc;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        text.text = "";
    }

    public static void CreateText(string _text, Color color, Directions direciton,
     float duration = 0, bool byPassPrevious = false, FontStyles fontStyle = default(FontStyles), float fontSize = _startFontSize, float outlineThickness = 0, System.Action action = null)
    {
        if (duration == 0) duration = direciton == Directions.TopToBottom ? Instance.topToBottomSpeed : Instance.xSpeed;
        Instance.StartCoroutine(Instance.TextWaitRoutine(_text, color, direciton, duration, byPassPrevious, fontStyle, fontSize, outlineThickness, action));
    }

    private IEnumerator TextWaitRoutine(string _text, Color color, Directions direction, float duration, bool byPassPrevious, FontStyles fontStyle, float fontSize, float outlineThickness, System.Action action = null)
    {
        while (flag)
        {
            if (byPassPrevious)
            {
                nextTextInc = true;
            }

            yield return null;
        }

        text.color = color;
        text.text = _text;
        text.fontSize = fontSize;
        text.fontStyle = fontStyle;
        text.fontSharedMaterial.SetFloat("_OutlineWidth", outlineThickness);
        Vector2 finalPos;

        switch (direction)
        {
            case Directions.TopToBottom:
                rectTransform.anchoredPosition = topToBottomStartPos;
                finalPos = topToBottomFinishPos;
                break;
            case Directions.RightToLeft:
                rectTransform.anchoredPosition = rightToLeftStartPos;
                finalPos = rightToLeftFinishPos;
                break;
            default:
                rectTransform.anchoredPosition = leftToRightStartPos;
                finalPos = leftToRightFinishPos;
                break;
        }

        flag = true;

        if (direction == Directions.TopToBottom)
            rectTransform.DOAnchorPos(finalPos, duration / 2).SetEase(topToBottomEase).OnComplete(() =>
             {
                 if (!nextTextInc)
                 {
                     var toColor = text.color;
                     toColor.a = 0;
                     text.DOColor(toColor, duration / 2).OnComplete(() => TweenEnded(action));
                 }
                 else
                     TweenEnded(action);
             }).SetUpdate(true);
        else
            rectTransform.DOAnchorPos(finalPos, duration).SetEase(xCurve).OnComplete(() => TweenEnded(action)).SetUpdate(true);
    }

    private void TweenEnded(System.Action action)
    {
        action?.Invoke();
        flag = false;
        text.text = "";
    }
}