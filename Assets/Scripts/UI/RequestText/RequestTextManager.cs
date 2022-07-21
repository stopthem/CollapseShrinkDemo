using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CanTemplate.Extensions;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using Object = System.Object;
using Random = UnityEngine.Random;

public class RequestTextManager : Singleton<RequestTextManager>
{
    [Header("General"), Space(5)] public List<Color> randomColors;
    private Pooler _requestTextPooler;

    protected override void Awake()
    {
        base.Awake();

        _requestTextPooler = GetComponentInChildren<Pooler>();
    }

    private readonly List<RequestText> _requestTexts = new();

    public static Sequence CreateText(RequestTextInfo requestTextInfo)
    {
        if (requestTextInfo.closePrevIfSameText)
            CloseSpecificText(requestTextInfo.text);

        return GetReqText().PlayText(requestTextInfo);
    }

    public static Sequence CreateText(RequestTextInfo requestTextInfo, out RequestText requestText)
    {
        if (requestTextInfo.closePrevIfSameText)
            CloseSpecificText(requestTextInfo.text);

        requestText = GetReqText();
        return requestText.PlayText(requestTextInfo);
    }

    ///<summary>It will create text with a random color.</summary>
    public static Sequence CreateText(RandomColoredRequestTextInfo randomColoredRequestTextInfo, out RequestText requestText)
    {
        if (randomColoredRequestTextInfo.closePrevIfSameText)
            CloseSpecificText(randomColoredRequestTextInfo.text);

        randomColoredRequestTextInfo.randomColors ??= Instance.randomColors;

        requestText = GetReqText();
        return requestText.PlayText(randomColoredRequestTextInfo);
    }

    ///<summary>It will create text with a random color.</summary>
    public static Sequence CreateText(RandomColoredRequestTextInfo randomColoredRequestTextInfo)
    {
        if (randomColoredRequestTextInfo.closePrevIfSameText)
            CloseSpecificText(randomColoredRequestTextInfo.text);

        randomColoredRequestTextInfo.randomColors ??= Instance.randomColors;

        return GetReqText().PlayText(randomColoredRequestTextInfo);
    }

    private static RequestText GetReqText()
    {
        var requestText = Instance._requestTextPooler.Get<RequestText>();
        Instance._requestTexts.Add(requestText);

        requestText.transform.SetParent(UIManager.instance.transform);
        requestText.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        requestText.transform.DOKill();
        return requestText;
    }

    public static void CloseAllRequestTexts(bool instant = true)
    {
        foreach (var reqText in Instance._requestTexts.Where(reqText => reqText != null))
        {
            reqText.Close(instant: instant);
        }

        Instance._requestTexts.Clear();
    }

    public static void CloseSpecificText(string text, bool instant = true) => CloseRequestText(Instance._requestTexts.FirstOrDefault(x => x.TMPText.text == text), instant);

    public static void CloseSpecificText(RequestTextInfo requestTextInfo, bool instant = false) => CloseRequestText(Instance._requestTexts.FirstOrDefault(x => x.MyReqTextInfo == requestTextInfo), instant);

    public static void CloseRequestText(RequestText requestText, bool instant)
    {
        if (requestText is null) return;
        requestText.Close(instant: instant);
        Instance._requestTexts.Remove(requestText);
    }
}

[Serializable]
public class RequestTextInfo
{
    public string text;

    [Tooltip("Leave it black with alpha 0 to set a random color")]
    public Color color = Color.white.WithA(1);

    public float tweenDuration = .5f;
    public float stayDuration = 4;

    public float fontSize = 80;

    public float playY = 600;

    public bool closePrevIfSameText = true;

    [CanBeNull] public UnderlayOptions underlayOptions = new()
    {
        dilate = .5f,
        offsetX = -.5f,
        offsetY = -.5f,
        softness = 0,
        underlayColor = Color.white.WithA(0)
    };
    [CanBeNull] public OutlineOptions outlineOptions = new()
    {
        color = Color.black.WithA(0),
        width = 0
    };
    public TMPro.FontStyles fontStyle;
}

[SerializeField]
public class RandomColoredRequestTextInfo : RequestTextInfo
{
    [CanBeNull] public List<Color> randomColors;
    public float colorA = 1;
}