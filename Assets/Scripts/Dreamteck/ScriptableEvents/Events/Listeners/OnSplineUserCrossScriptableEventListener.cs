using ScriptableEvents;
using Dreamteck.Splines;
using UnityEngine;

namespace CanTemplate.Events.Listeners
{
    [AddComponentMenu(
        ScriptableEventConstants.MenuNameCustom + "/On Spline User Cross Scriptable Event Listener",
        ScriptableEventConstants.MenuOrderCustom + 0
    )]
    public class OnSplineUserCrossScriptableEventListener : BaseScriptableEventListener<SplineUser>
    {
    }
}
