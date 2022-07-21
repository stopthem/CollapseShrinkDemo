using System;
using System.Collections;
using System.Collections.Generic;
using CanTemplate;
using Coffee.UIEffects;
using DG.Tweening;
using UnityEngine;
using DG.DeInspektor.Attributes;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using CanTemplate.Extensions;
using ScriptableEvents.Events;

public class AnimatedButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private bool canInteract = true;

    [SerializeField, Space(5)] private StartingBehaviour startingBehaviour;
    [SerializeField] private bool noInteractAfterClicked = true;

    [Header("Open and close tween options"), Space(5), SerializeField]
    private TweenOptionsDEase openCloseTweenOptions = new()
    {
        duration = .25f,
        ease = Ease.OutBack,
        secondEase = Ease.InBack
    };

    [Header("Clicked tween options"), Space(5)] [SerializeField]
    private FTweenOptions clickedFTweenOptions = new()
    {
        duration = .15f,
        ease = Ease.InOutSine,
        target = .9f
    };

    public UnityEvent onClick;

    public bool IsOpen { get; private set; }

    private Tween _scaleTween;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        switch (startingBehaviour)
        {
            case StartingBehaviour.StartOpen:
                break;
            case StartingBehaviour.StartClosed:
                InstantClose();
                break;
            case StartingBehaviour.StartClosedThenOpen:
                InstantClose();
                Open();
                break;
        }
    }

    public void Open()
    {
        if (!IsOpen)
            OpenOrClose(true);
    }

    private void ResetScale(float toScale)
    {
        transform.DOKill();
        transform.localScale = Vector3.one * toScale;
    }

    private void OpenOrClose(bool open)
    {
        float toScale = open ? 1 : 0;
        ResetScale(open ? 0 : 1);

        IsOpen = open;

        if (!open) ChangeInteract(false);

        _scaleTween = transform.DOScale(toScale, openCloseTweenOptions.duration).SetEase(open ? openCloseTweenOptions.ease : openCloseTweenOptions.secondEase)
                               .OnComplete(() =>
                               {
                                   if (open) ChangeInteract(true);
                               });
    }

    public void Close(bool instant = false)
    {
        if (!IsOpen) return;

        if (instant)
            InstantClose();
        else
            OpenOrClose(false);
    }

    private void InstantClose()
    {
        IsOpen = false;
        transform.localScale = Vector3.zero;
    }

    public void ClickedTween()
    {
        if (_scaleTween.IsActive() && _scaleTween.IsPlaying()) return;
        ResetScale(1);
        if (_animator) _animator.enabled = false;
        _scaleTween = transform.DOScale(clickedFTweenOptions.target, clickedFTweenOptions.duration / 2).SetEase(clickedFTweenOptions.ease).SetLoops(2, LoopType.Yoyo).SetUpdate(true).OnComplete(() =>
        {
            if (_animator)
            {
                _animator.enabled = true;
            }
        });
    }

    public void ChangeInteract(bool status) => canInteract = status;

    private void Click()
    {
        if (!canInteract) return;

        if (noInteractAfterClicked) ChangeInteract(false);

        ClickedTween();
        onClick.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Click();
    }

    private enum StartingBehaviour
    {
        StartOpen,
        StartClosed,
        StartClosedThenOpen
    }
}