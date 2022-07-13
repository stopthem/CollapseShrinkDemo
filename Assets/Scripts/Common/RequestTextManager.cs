using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CanTemplate.Extensions;
using DG.Tweening;
using UnityEngine;

public class RequestTextManager : Singleton<RequestTextManager>
{
    private Pooler _requestTextPooler;
    [SerializeField] private List<Color> randomColors;

    protected override void Awake()
    {
        base.Awake();

        _requestTextPooler = GetComponentInChildren<Pooler>();
    }

    private List<RequestText> _requestTexts = new List<RequestText>();

    ///<param name ="playY">Where to play the text in anchored y.</param>
    public static Sequence CreateText(string text, Color textColor, float tweenDuration = .5f, float stayDuration = 4, float fontSize = 80,
        float playY = 600, bool underlay = true, bool closeIfHasSameText = true)
    {
        if (closeIfHasSameText)
            CloseSpecificText(text, true);

        return GetReqText().PlayText(text, textColor, tweenDuration, stayDuration, fontSize, playY, underlay);
    }

    ///<summary>It will create text with a random color.</summary>
    ///<param name ="playY">Where to play the text in anchored y.</param>
    ///<param name = "textRandomColors">If null, this scripts randomColors will be used.</param>
    public static Sequence CreateText(string text, float tweenDuration = .5f, float stayDuration = 4, float fontSize = 80,
        float playY = 600, bool underlay = true, List<Color> textRandomColors = null, float colorA = 1, bool closeIfHasSameText = true)
    {
        if (closeIfHasSameText)
            CloseSpecificText(text, true);
        var randomColorsList = textRandomColors ?? Instance.randomColors;
        return GetReqText().PlayText(text, randomColorsList[Random.Range(0, randomColorsList.Count)].WithA(colorA), tweenDuration, stayDuration, fontSize, playY, underlay);
    }

    private static RequestText GetReqText()
    {
        var requestText = Instance._requestTextPooler.GetObject().GetComponent<RequestText>();
        Instance._requestTexts.Add(requestText);

        requestText.transform.SetParent(UIManager.Instance.transform);
        requestText.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        requestText.transform.DOKill();
        return requestText;
    }

    public static void CloseAllReqTexts()
    {
        foreach (var reqText in Instance._requestTexts)
        {
            if (reqText != null) reqText.Close();
        }

        Instance._requestTexts.Clear();
    }

    public static void CloseSpecificText(string text, bool instant = false)
    {
        var foundReqText = Instance._requestTexts.FirstOrDefault(x => x.tmpText.text == text);
        if (foundReqText != null)
        {
            foundReqText.Close(instant: instant);
            Instance._requestTexts.Remove(foundReqText);
        }
    }
}