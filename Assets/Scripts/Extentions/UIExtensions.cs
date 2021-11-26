using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CanTemplate.Extensions
{
    public static class UIExtensions
    {
        public static void UpdateLayout(this LayoutGroup layoutGroup)
        {
            Canvas.ForceUpdateCanvases();
            layoutGroup.enabled = false;
            layoutGroup.enabled = true;
        }
    }
}
