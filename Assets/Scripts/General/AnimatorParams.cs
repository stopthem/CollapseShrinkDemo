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

        public static readonly int RunId = Animator.StringToHash("Run");

        public static readonly int CastingId = Animator.StringToHash("Casting");

        public static readonly int FallId = Animator.StringToHash("Fall");

        public static readonly int DanceId = Animator.StringToHash("Dance");

        public static readonly int OnCubeId = Animator.StringToHash("OnCube");

        public static readonly int IdleId = Animator.StringToHash("Idle");

        public static readonly int StunId = Animator.StringToHash("Stun");
    }
}