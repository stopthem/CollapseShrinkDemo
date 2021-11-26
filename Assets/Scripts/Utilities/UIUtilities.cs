using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace CanTemplate.Utils
{
    public static class UIUtilities
    {
        ///<param name = "rootTransform">Looks at this transform and its childs. Default is UIManager's transform.</param>
        public static bool IsPointerOverUIObject(Transform rootTransform = null)
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            if (rootTransform)
            {
                var childList = rootTransform.GetComponentsInChildren<RectTransform>().ToList();
                results.RemoveAll(x => !childList.Contains(x.gameObject.transform));
            }
            return results.Count > 0;
        }
    }
}


