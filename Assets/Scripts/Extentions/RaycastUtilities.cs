using System;
using CanTemplate.Camera;
using CanTemplate.Input;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace CanTemplate.Utilities
{
    public static class RaycastUtilities
    {
        #region RaycastAll

        public static RaycastHit[] RaycastAllHitFromCam(float rayLength = Mathf.Infinity, bool checkOverUI = false, LayerMask? layerMask = null)
        {
            if (checkOverUI && UIUtilities.IsPointerOverUIObject())
                return null;

            return Physics.RaycastAll(CinemachineManager.CamToTouchRay, rayLength, layerMask ?? Physics.AllLayers);
        }

        public static RaycastHit[] RaycastAllHitFromCamNonAlloc(int raycastHitLength = 10, float rayLength = Mathf.Infinity, bool checkOverUI = false, LayerMask? layerMask = null)
        {
            if (checkOverUI && UIUtilities.IsPointerOverUIObject())
                return null;

            var raycastHits = new RaycastHit[raycastHitLength];
            Physics.RaycastNonAlloc(CinemachineManager.CamToTouchRay, raycastHits, rayLength, layerMask ?? Physics.AllLayers);

            return raycastHits;
        }

        #endregion

        public static RaycastHit RayHitFromCam(float rayLength = Mathf.Infinity, bool checkOverUI = false, LayerMask? layerMask = null)
        {
            if (checkOverUI && UIUtilities.IsPointerOverUIObject())
                return new RaycastHit();

            var ray = CinemachineManager.MainCam.ScreenPointToRay(InputManager.GetLastTouchPos);
            return Physics.Raycast(ray, out var hit, rayLength, layerMask ?? Physics.AllLayers) ? hit : new RaycastHit();
        }

        public static T RayHitFromCam<T>(float rayLength = Mathf.Infinity, bool checkOverUI = false, LayerMask? layerMask = null)
        {
            if (checkOverUI && UIUtilities.IsPointerOverUIObject())
            {
                return default;
            }

            return Physics.Raycast(CinemachineManager.CamToTouchRay, out var hit, rayLength, layerMask ?? Physics.AllLayers) ? hit.collider.GetComponent<T>() : default;
        }
    }
}