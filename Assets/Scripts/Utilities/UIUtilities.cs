using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.InputSystem;

namespace CanTemplate.Utilities
{
    public static class UIUtilities
    {
        ///<param name = "rootTransform">Looks at this transform and its childs. If null, looks at all ui objects.</param>
        public static bool IsPointerOverUIObject(Transform rootTransform = null)
        {
            var uiObjs = GetUIObjectsInPointerPos();
            
            if (!rootTransform) return uiObjs.Count > 0;
            
            var childList = rootTransform.GetComponentsInChildren<RectTransform>().ToList();
            uiObjs.RemoveAll(x => !childList.Contains(x.gameObject.transform));

            return uiObjs.Count > 0;
        }

        public static List<GameObject> GetUIObjectsInPointerPos()
        {
            var eventDataCurrentPosition = new PointerEventData(EventSystem.current)
            {
                position = Touchscreen.current.position.ReadValue()
            };
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

            var resultObjects = results.Select(x => x.gameObject).ToList();
            return resultObjects;
        }

        /// <summary>
        /// Opens or closes the given named GameObject who is a child of UIManager in the hierarchy
        /// </summary>
        /// <param name="name"></param>
        /// <param name="status"></param>
        /// <param name="updateChildsList">This list is filled in the Start, If True: updates the childs list</param>
        public static void OpenClose(string name, bool status, bool updateChildsList = false)
        {
            if (updateChildsList) UIManager.instance.childs = UIManager.instance.GetComponentsInChildren<RectTransform>(true).ToList();

            var obj = UIManager.instance.childs.FirstOrDefault(x => x.name == name);

            if (!obj)
            {
                Debug.Log("ui manager couldn't find = " + "<color=green>" + name + "</color>");
                return;
            }

            obj.gameObject.SetActive(status);
        }

        /// <summary>
        /// Opens or closes the given named GameObject who is a child of UIManager in the hierarchy
        /// </summary>
        /// <param name="uiObj"></param>
        /// <param name="status"></param>
        /// <param name="updateChildsList">This list is filled in the Start, If True: updates the childs list</param>
        public static void OpenClose(GameObject uiObj, bool status, bool updateChildsList = false)
        {
            if (updateChildsList) UIManager.instance.childs = UIManager.instance.GetComponentsInChildren<RectTransform>(true).ToList();

            var obj = UIManager.instance.childs.FirstOrDefault(x => x.gameObject == uiObj);

            if (!obj)
            {
                Debug.Log("ui manager couldn't find = " + "<color=green>" + uiObj + "</color>");
                return;
            }

            obj.gameObject.SetActive(status);
        }
    }
}