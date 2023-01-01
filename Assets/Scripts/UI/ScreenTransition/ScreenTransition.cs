using CanTemplate;
using CanTemplate.Extensions;
using CanTemplate.UI;
using DG.Tweening;
using UnityEngine;

public class ScreenTransition : MonoBehaviour
{
    [SerializeField] private RectTransform maskImage, maskBc;
    [SerializeField] private Vector2 toSize;

    [SerializeField] private TweenOptions openCloseTweenOptions = new()
    {
        duration = 1,
        easeSelection = new EaseSelection(Ease.InOutExpo)
    };

    private void Start()
    {
        maskBc.SetParent(UIManager.Instance.transform);
        maskBc.sizeDelta = new Vector2(Screen.width, Screen.height);
        maskBc.SetParent(maskImage);
        maskImage.sizeDelta = toSize;
    }

    public Tween OpenClose(bool status)
    {
        maskImage.sizeDelta = status ? toSize : Vector2.zero;

        return maskImage.DOSizeDelta(status ? Vector2.zero : toSize, openCloseTweenOptions.duration).SetEase(openCloseTweenOptions.easeSelection).SetUpdate(status);
    }
}