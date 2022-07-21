using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace CanTemplate.Utilities
{
    public static class InputUtilities
    {
        ///<summary>Creates a primaryTouch tap and checks it until the returned inputAction is disabled.</summary>
        public static InputAction Tap(System.Action<InputAction.CallbackContext> onSuccessfulClick, bool disableOnLevelEnd = true)
        {
            var tapAction = new InputAction(binding: "<Touchscreen>/primaryTouch/tap");
            tapAction.Enable();
            tapAction.performed += onSuccessfulClick.Invoke;

            GameManager.instance.onGameEnded.AddListener(() => tapAction.Disable());

            return tapAction;
        }

        public static bool TryGetSingleTouch(out Touch touch)
        {
            if (Touch.activeTouches.Count == 1)
            {
                touch = Touch.activeTouches.First();
                return true;
            }

            touch = new Touch();
            return false;
        }
    }
}