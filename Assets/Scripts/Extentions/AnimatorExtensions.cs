using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CanTemplate.Extensions
{
    public static class AnimatorExtensions
    {
        public static List<string> GetParametersWithName(this Animator animator, string name, AnimatorControllerParameterType animatorControllerParameterType = AnimatorControllerParameterType.Trigger)
            => animator.parameters.Where(x => x.name.Contains(name) && x.type == animatorControllerParameterType).Select(x => x.name).ToList();
    }
}