using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using CanTemplate.Extensions;

public class RequestText : MonoBehaviour
{
    public static RequestText Instance;

    private TextMeshProUGUI _requestText;
    [SerializeField] private float stayDuration = 4;
    [SerializeField] private float tweenDuration = .5f;
    [SerializeField, Range(0, 1)] private float darkerPercent = .75f;
    [SerializeField] private EaseSelection inEase = new EaseSelection(Ease.OutBack), outEase = new EaseSelection(Ease.OutQuart);

    private Sequence _seq;

    private HorizontalLayoutGroup _horizontalLayoutGroup;

    private void Awake()
    {
        Instance = this;
        _requestText = GetComponent<TextMeshProUGUI>();
        _requestText.fontSize = 0;
    }

    public void CreateText(string text, Color? textColor = null, float tweenDuration = 0, float stayDuration = 0, float fontSize = 60, EaseSelection _inEase = null, EaseSelection _outEase = null)
    {
        tweenDuration = tweenDuration == 0 ? this.tweenDuration : tweenDuration;
        stayDuration = stayDuration == 0 ? this.stayDuration : stayDuration;

        _requestText.text = text;

        Color _textColor = textColor.HasValue ? textColor.Value : Color.white;
        _requestText.color = _textColor;

        _requestText.fontSize = 0;
        _requestText.fontSharedMaterial.SetColor("_UnderlayColor", new Color(_textColor.r * darkerPercent, _textColor.g * darkerPercent, _textColor.b * darkerPercent));

        if (_seq.IsActive() && _seq.IsPlaying()) _seq.Kill();

        _seq = DOTween.Sequence()
        .Append(DOTween.To(x => _requestText.fontSize = x, 0, fontSize, tweenDuration / 2).SetEase(_inEase == null ? inEase : _inEase))
        .AppendInterval(stayDuration)
        .Append(DOTween.To(x => _requestText.fontSize = x, fontSize, fontSize * 1.15f, tweenDuration / 2).SetEase(_outEase == null ? outEase : _outEase))
        .Join(_requestText.DOColor(_requestText.color.WithA(0), tweenDuration / 2).SetEase(_outEase == null ? outEase : _outEase));
    }

    public void CloseCurrentText(System.Action action = null)
    {
        if (transform.localScale == Vector3.zero)
        {
            action?.Invoke();
            return;
        }
        _seq.Kill();
        transform.DOScale(Vector3.zero, tweenDuration).SetEase(outEase).OnComplete(() => action?.Invoke());
    }
}
