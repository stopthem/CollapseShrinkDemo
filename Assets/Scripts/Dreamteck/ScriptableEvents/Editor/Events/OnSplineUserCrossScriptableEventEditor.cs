using ScriptableEvents.Editor;
using CanTemplate.Events;
using Dreamteck.Splines;
using UnityEditor;

namespace CanTemplate.Editor.Events
{
    [CustomEditor(typeof(OnSplineUserCrossScriptableEvent))]
    public class OnSplineUserCrossScriptableEventEditor : BaseScriptableEventEditor<SplineUser>
    {
        protected override SplineUser DrawArgField(SplineUser value)
        {
            // Use EditorGUILayout.TextField, etc., to draw inputs next to Raise button on the
            // OnSplineUserCrossScriptableEvent asset.
            return value;
        }
    }
}
