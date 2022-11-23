using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CanTemplate.Input
{
    #if UNITY_EDITOR

    [InitializeOnLoad]
#endif
    public class ViewportVector2 : InputProcessor<Vector2>
    {
#if UNITY_EDITOR
        static ViewportVector2()
        {
            Initialize();
        }
#endif

        [RuntimeInitializeOnLoadMethod]
        public static void Initialize()
        {
            InputSystem.RegisterProcessor<ViewportVector2>();
        }

        public override Vector2 Process(Vector2 value, InputControl control) => new(value.x / Screen.width, value.y / Screen.height);
    }
}