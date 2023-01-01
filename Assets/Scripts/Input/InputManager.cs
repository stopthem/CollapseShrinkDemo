using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

namespace CanTemplate.Input
{
    public class InputManager : MonoBehaviour
    {
        private static TouchControls _touchControls;

        public static InputManager Instance { get; private set; }

        private readonly List<InputInfo> _inputInfos = new();

        /// <summary>
        /// Difference and direction between last frame touch position and this current frame touch position
        /// </summary>
        public Vector2 TouchDelta => _touchControls.TouchMap.TouchDelta.ReadValue<Vector2>();

        public static Action<InputAction.CallbackContext> onTouchBegin, onTouchTap, onTouchHold, onTouchEnd;

        public static Vector2 GetLastTouchPos => _touchControls.TouchMap.TouchPosition.ReadValue<Vector2>();

        private void Awake()
        {
            Instance = this;

            _touchControls = new TouchControls();
        }

        private void OnEnable()
        {
            EnhancedTouchSupport.Enable();
            TouchSimulation.Enable();
            _touchControls.Enable();
        }

        private void OnDisable()
        {
            EnhancedTouchSupport.Disable();
            TouchSimulation.Disable();
            _touchControls.Disable();
        }

        private void Start()
        {
            _touchControls.TouchMap.TouchHold.started += ctx => onTouchBegin?.Invoke(ctx);
            _touchControls.TouchMap.TouchTap.performed += ctx => onTouchTap?.Invoke(ctx);
            _touchControls.TouchMap.TouchHold.canceled += ctx =>
            {
                onTouchEnd?.Invoke(ctx);

                ResetInputs();
            };

            _touchControls.TouchMap.TouchHold.performed += ctx =>
            {
                onTouchHold?.Invoke(ctx);

                SetInputInfos();
            };
        }

        private void ResetInputs()
        {
            CheckInputInfos();
        }

        private void SetInputInfos()
        {
            foreach (var inputInfo in _inputInfos)
                inputInfo.SetForcedValue(TouchDelta);
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
        public InputInfo(float speed, bool canGetReset)
        {
            this.speed = speed;
            this.canGetReset = canGetReset;
        }

        public readonly bool canGetReset;
        protected readonly float speed;

        protected bool update = true;

        public abstract void SetIncrementalValue(Vector2 difference, bool useSpeed = true);
        public abstract void SetForcedValue(Vector2 addedVector, bool useSpeed = true);

        public void HandleUpdate(bool status) => update = status;
    }

    public class FInputInfo : InputInfo
    {
        public FInputInfo(float speed, bool canGetReset) : base(speed, canGetReset)
        {
        }

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
        public VInputInfo(float speed, bool canGetReset) : base(speed, canGetReset)
        {
        }

        public float X { get; private set; }
        public float Y { get; private set; }

        public override void SetIncrementalValue(Vector2 difference, bool useSpeed = true)
        {
            if (!update) return;
            X += difference.x * (useSpeed ? speed : 1);
            Y += difference.y * (useSpeed ? speed : 1);
        }

        public override void SetForcedValue(Vector2 addedVector, bool useSpeed = true)
        {
            X = addedVector.x * (useSpeed ? speed : 1);
            Y = addedVector.y * (useSpeed ? speed : 1);
        }
    }
}