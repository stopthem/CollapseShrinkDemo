using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CanTemplate.Utilities;
using DG.DeInspektor.Attributes;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class PlayerInput : MonoBehaviour
{
    private Vector3 _prevFrameMousePos;

    private readonly List<InputInfo> _inputInfos = new();

    public Vector3 Direction { get; private set; }

    private void Awake()
    {
        EnhancedTouchSupport.Enable();
        TouchSimulation.Enable();
    }

    private void Update()
    {
        if (GameManager.gameStatus != GameManager.GameStatus.Play || !InputUtilities.TryGetSingleTouch(out var touch))
        {
            ResetInputs();
            return;
        }

        if (touch.inProgress)
        {
            var currentMousePos = Camera.main.ScreenToViewportPoint(touch.screenPosition);
            if (_prevFrameMousePos != Vector3.zero)
            {
                Direction = currentMousePos - _prevFrameMousePos;
                SetInputInfos();
            }

            _prevFrameMousePos = currentMousePos;
        }
        else if (touch.ended)
        {
            ResetInputs();
        }
    }

    private void ResetInputs()
    {
        CheckInputInfos();

        _prevFrameMousePos = Vector3.zero;
        Direction = Vector3.zero;
    }

    private void SetInputInfos()
    {
        foreach (var inputInfo in _inputInfos)
            inputInfo.SetForcedValue(Direction);
    }

    private void CheckInputInfos()
    {
        foreach (var inputInfo in _inputInfos.Where(x => x.canGetReset))
            inputInfo.SetForcedValue(Vector2.zero);
    }

    public void NewInputInfo(InputInfo inputInfo) => _inputInfos.Add(inputInfo);
}

public abstract class InputInfo
{
    public bool canGetReset;
    public float speed;

    protected bool update = true;

    public abstract void SetIncrementalValue(Vector2 difference, bool useSpeed = true);
    public abstract void SetForcedValue(Vector2 addedVector, bool useSpeed = true);

    public void HandleUpdate(bool status) => update = status;
}

public class FInputInfo : InputInfo
{
    public bool xAxis = true;

    public float Value { get; private set; }

    public Vector2 Direction { get; private set; }

    public override void SetIncrementalValue(Vector2 difference, bool useSpeed = true)
    {
        if (!update) return;
        Direction = difference;
        Value += (xAxis ? difference.x : difference.y) * (useSpeed ? speed : 1);
    }

    public override void SetForcedValue(Vector2 addedVector, bool useSpeed = true)
    {
        Direction = addedVector;
        var addedVal = xAxis ? addedVector.x : addedVector.y;
        addedVal *= useSpeed ? speed : 1;
        Value = addedVal;
    }
}

public class VInputInfo : InputInfo
{
    public float x { get; private set; }
    public float y { get; private set; }

    public override void SetIncrementalValue(Vector2 difference, bool useSpeed = true)
    {
        if (!update) return;
        x += difference.x * (useSpeed ? speed : 1);
        y += difference.y * (useSpeed ? speed : 1);
    }

    public override void SetForcedValue(Vector2 addedVector, bool useSpeed = true)
    {
        x = addedVector.x * (useSpeed ? speed : 1);
        y = addedVector.y * (useSpeed ? speed : 1);
    }
}