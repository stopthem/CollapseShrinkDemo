using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace CanTemplate.Utils
{
    public static class UIUtilities
    {
        ///<param name = "rootTransform">Looks at this transform and its childs. If null, looks at all ui objects.</param>
        public static bool IsPointerOverUIObject(Transform rootTransform = null)
        {
            var uiObjs = GetMousePosUI();
            if (rootTransform)
            {
                var childList = rootTransform.GetComponentsInChildren<RectTransform>().ToList();
                uiObjs.RemoveAll(x => !childList.Contains(x.gameObject.transform));
            }

            return uiObjs.Count > 0;
        }

        public static List<GameObject> GetMousePosUI()
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

            var resultObjects = results.Select(x => x.gameObject).ToList();
            return resultObjects;
        }
    }
}