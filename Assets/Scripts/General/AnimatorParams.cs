using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeShooter
{
    public static class AnimatorParams
    {
        public static readonly int CloseId = Animator.StringToHash("Close");

        public static readonly int OpenId = Animator.StringToHash("Open");

        public static readonly int ClickId = Animator.StringToHash("Click");
    }
}