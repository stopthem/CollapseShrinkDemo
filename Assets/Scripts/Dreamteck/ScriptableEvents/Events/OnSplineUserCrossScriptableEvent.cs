using ScriptableEvents;
using Dreamteck.Splines;
using UnityEngine;

namespace CanTemplate.Events
{
    [CreateAssetMenu(
        fileName = "OnSplineUserCrossScriptableEvent",
        menuName = ScriptableEventConstants.MenuNameCustom + "/On Spline User Cross Scriptable Event",
        order = ScriptableEventConstants.MenuOrderCustom + 0
    )]
    public class OnSplineUserCrossScriptableEvent : BaseScriptableEvent<SplineUser>
    {
    }
}
