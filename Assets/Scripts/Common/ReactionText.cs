using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    [SerializeField] private AnimationCurve classicCurve;
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
     float speed = 0, bool byPassPrevious = false, FontStyles fontStyle = default(FontStyles), float fontSize = _startFontSize, float outlineThickness = 0, System.Action action = null)
    {
        if (speed == 0) speed = direciton == Directions.TopToBottom ? Instance.topToBottomSpeed : Instance.xSpeed;
        Instance.StartCoroutine(Instance.TextWaitRoutine(_text, color, direciton, speed, byPassPrevious, fontStyle, fontSize, outlineThickness, action));
    }

    private IEnumerator TextWaitRoutine(string _text, Color color, Directions direciton, float speed, bool byPassPrevious, FontStyles fontStyle, float fontSize, float outlineThickness, System.Action action = null)
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
        Vector2 startPos;
        Vector2 finalPos;
        AnimationCurve curve = xCurve;

        if (direciton == Directions.TopToBottom)
        {
            curve = classicCurve;
            startPos = topToBottomStartPos;
            finalPos = topToBottomFinishPos;
        }
        else if (direciton == Directions.RightToLeft)
        {
            startPos = rightToLeftStartPos;
            finalPos = rightToLeftFinishPos;
        }
        else
        {
            startPos = leftToRightStartPos;
            finalPos = leftToRightFinishPos;
        }

        StartCoroutine(Move(startPos, finalPos, curve, speed, action));
    }

    private IEnumerator Move(Vector2 startPos, Vector2 finalPos, AnimationCurve curve, float speed, System.Action action = null)
    {
        flag = true;
        rectTransform.anchoredPosition = startPos;

        var timer = 0f;
        while (timer < 1)
        {
            if (nextTextInc)
            {
                nextTextInc = false;
                break;
            }
            timer += Time.deltaTime * speed;
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, finalPos, curve.Evaluate(timer));
            yield return null;
        }


        if (startPos == topToBottomStartPos && !nextTextInc)
        {
            timer = 0f;
            while (timer < 1)
            {
                if (nextTextInc)
                {
                    nextTextInc = false;
                    break;
                }

                timer += Time.deltaTime * speed;
                var color = text.color;
                color.a = Mathf.Lerp(1, 0, timer);
                text.color = color;
                yield return null;
            }
        }

        if (action != null)
        {
            action.Invoke();
        }

        flag = false;
        text.text = "";
    }

    private Vector2 SmoothVector2Lerp(Vector2 begin, Vector2 end, float t)
    {
        var startYvalue = begin.y;
        var endYValue = end.y;
        var startXValue = begin.x;
        var endXValue = end.x;
        var currentYValue = Mathf.SmoothStep(startYvalue, endYValue, t);
        var currentXValue = Mathf.SmoothStep(startXValue, endXValue, t);
        return new Vector2(currentXValue, currentYValue);
    }
}