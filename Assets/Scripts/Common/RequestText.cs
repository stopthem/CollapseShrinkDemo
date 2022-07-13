using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using CanTemplate.Extensions;
using System.Linq;

public class RequestText : MonoBehaviour
{
    public TextMeshProUGUI tmpText { get; private set; }
    [SerializeField, Range(0, 1)] private float underlayDarkerPercent = .75f, outlineDarkerPercent = .5f;
    [SerializeField] private EaseSelection inEase = new EaseSelection(Ease.OutBack), outEase = new EaseSelection(Ease.OutQuart);
    [SerializeField] private float justCloseDuration = .25f;

    private RectTransform _rectTransform;

    private Sequence _seq;

    private HorizontalLayoutGroup _horizontalLayoutGroup;

    private Poolable _poolable;

    private void Awake()
    {
        tmpText = GetComponent<TextMeshProUGUI>();
        tmpText.fontSize = 0;
        _rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        _poolable = GetComponent<Poolable>();
    }

    public Sequence PlayText(string text, Color? textColor, float tweenDuration, float stayDuration, float fontSize, float playY, bool underlay)
    {
        _rectTransform.anchoredPosition = _rectTransform.anchoredPosition.WithY(playY);

        tmpText.text = text;

        Color actualColor = textColor ?? Color.white;
        tmpText.color = actualColor;
        tmpText.fontSize = fontSize;

        Close(instant: true);

        Color underlayColor = actualColor * underlayDarkerPercent;
        underlayColor = underlayColor.WithA(underlay ? .75f : 0f);
        tmpText.fontMaterial.SetColor("_UnderlayColor", underlayColor);
        tmpText.fontMaterial.SetColor("_OutlineColor", actualColor * outlineDarkerPercent);

        return _seq = DOTween.Sequence()
            .Append(transform.DOScale(Vector3.one, tweenDuration / 2).SetEase(inEase))
            .AppendInterval(stayDuration)
            .Append(CloseSeq(tweenDuration))
            .SetUpdate(true);
    }

    public void Close(TweenCallback callback = null, bool instant = false)
    {
        if (_seq.IsActive() && _seq.IsPlaying())
            _seq.Kill();

        if (instant)
            transform.localScale = Vector3.zero;
        else
            CloseSeq(justCloseDuration).OnComplete(callback);
    }

    private Sequence CloseSeq(float duration)
    {
        return DOTween.Sequence()
            .Append(transform.DOScale(5f, duration / 2).SetEase(outEase))
            .Join(tmpText.DOColor(tmpText.color.WithA(0), duration / 2).SetEase(outEase))
            .AppendCallback(() => _poolable.ClearMe())
            .SetUpdate(true);
    }
}