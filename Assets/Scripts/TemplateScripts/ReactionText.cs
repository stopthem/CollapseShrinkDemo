using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ReactionText : Singleton<ReactionText>
{
    private static ReactionText instance;
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

    private bool flag = false;
    private bool nextTextInc;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        text.text = "";
    }

    public static void CreateText(string _text, Color color, Directions direciton, bool byPassPrevious = false, System.Action action = null)
    {
        instance.StartCoroutine(instance.TextWaitRoutine(_text, color, direciton, byPassPrevious, action));
    }

    private IEnumerator TextWaitRoutine(string _text, Color color, Directions direciton, bool byPassPrevious, System.Action action = null)
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
        Vector2 startPos;
        Vector2 finalPos;
        AnimationCurve curve = xCurve;
        float speed = xSpeed;

        if (direciton == Directions.TopToBottom)
        {
            curve = classicCurve;
            speed = topToBottomSpeed;
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
