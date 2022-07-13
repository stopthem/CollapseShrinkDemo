using System;
using System.Collections;
using System.Collections.Generic;
using CanTemplate.Extensions;
using DG.Tweening;
using UnityEngine;

public class SphereLikeMover : MonoBehaviour
{
    public SphereLikeMover otherSphereLikeMover;
    [SerializeField] private float detectMovingMin = .1f;
    private Vector3 _dirBetweenMousePoses;
    public bool IsMoving { get => _dirBetweenMousePoses.magnitude >= detectMovingMin; }
    [SerializeField] private float rotateSpeed = 1.5f;
    [SerializeField, MinMaxSlider(-90, 90)] private Vector2 xRotRange;
    [SerializeField, MinMaxSlider(-90, 90)] private Vector2 yRotRange;
    private Tuple<float, float> _rotateNormalized;
    public Tuple<float, float> _startRotateNormalized { get; private set; }
    public Tuple<float, float> RotateNormalized
    {
        get => _rotateNormalized;
        private set
        {
            var item1 = Mathf.Clamp01(value.Item1);
            var item2 = Mathf.Clamp01(value.Item2);

            _rotateNormalized = Tuple.Create(item1, item2);

            var toRot = Quaternion.Euler(Mathf.Lerp(xRotRange.x, xRotRange.y, item1),
             Mathf.Lerp(yRotRange.x, yRotRange.y, item2), 0);
            toRot.eulerAngles = toRot.ClampEuler();
            transform.localRotation = toRot;
        }
    }
    private Vector3 _currentMousePos;
    [HideInInspector] public bool canMove;

    public System.Action onMouseUp;

    private void Start()
    {
        canMove = false;
        RotateNormalized = _startRotateNormalized = Tuple.Create(Mathf.InverseLerp(xRotRange.x, xRotRange.y, transform.localRotation.ClampEuler().x)
        , Mathf.InverseLerp(yRotRange.x, yRotRange.y, transform.localRotation.ClampEuler().y));
    }

    private void Update()
    {
        if (canMove)
        {
            if (Input.GetMouseButton(0))
            {
                Move();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                onMouseUp?.Invoke();
                _currentMousePos = Vector3.zero;
            }
        }
    }

    private void Move()
    {
        if (_currentMousePos == Vector3.zero)
        {
            _currentMousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            _dirBetweenMousePoses = Vector3.zero;
            return;
        }
        if (Camera.main.ScreenToViewportPoint(Input.mousePosition) == _currentMousePos)
        {
            _dirBetweenMousePoses = Vector3.zero;
            return;
        }

        _dirBetweenMousePoses = Camera.main.ScreenToViewportPoint(Input.mousePosition) - _currentMousePos;
        var xDifference = _dirBetweenMousePoses.x * rotateSpeed;
        var yDifference = _dirBetweenMousePoses.y * rotateSpeed;
        SetRotateNormalized(Tuple.Create(RotateNormalized.Item1 + -yDifference, RotateNormalized.Item2 + -xDifference), false, otherSphereLikeMover);
        _currentMousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
    }

    public void SetRotateNormalized(Tuple<float, float> val, bool smooth = false, SphereLikeMover otherSphereLikeMover = null, bool bypassCanMove = false, System.Action action = null)
    {
        if (!smooth && (canMove || bypassCanMove))
        {
            if (otherSphereLikeMover)
                otherSphereLikeMover.SetRotateNormalized(val, bypassCanMove: true);

            RotateNormalized = val;
        }
        else if (smooth)
        {
            float dum1 = RotateNormalized.Item1, dum2 = RotateNormalized.Item2;
            DOTween.Sequence()
            .Append(DOTween.To(x => dum1 = x, dum1, val.Item1, .5f))
            .Join(DOTween.To(x => dum2 = x, dum2, val.Item2, .5f))
            .OnUpdate(() =>
            {
                Tuple<float, float> tuple = Tuple.Create(dum1, dum2);
                RotateNormalized = tuple;

                if (otherSphereLikeMover)
                    otherSphereLikeMover.SetRotateNormalized(tuple, bypassCanMove: true);

            }).AppendCallback(() =>
            {
                canMove = true;
                action?.Invoke();
            });
        }
    }

    public void ResetPos(System.Action action = null)
    {
        _currentMousePos = Vector3.zero;
        canMove = false;
        float dum1 = RotateNormalized.Item1, dum2 = RotateNormalized.Item2;
        DOTween.Sequence()
        .Append(DOTween.To(x => dum1 = x, dum1, _startRotateNormalized.Item1, .5f))
        .Join(DOTween.To(x => dum2 = x, dum2, _startRotateNormalized.Item2, .5f))
        .OnUpdate(() => RotateNormalized = Tuple.Create(dum1, dum2)).AppendCallback(() => action?.Invoke());
    }
}
