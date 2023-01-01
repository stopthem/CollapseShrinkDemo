using UnityEngine;

namespace CanTemplate.Camera
{
    public class LookAtCam : MonoBehaviour
    {
        [SerializeField] private UpdateMode updateMode;

        private enum UpdateMode
        {
            Update,
            Fixed,
            LateUpdate
        }

        private void FixedUpdate()
        {
            if (updateMode is UpdateMode.Fixed) LookAt();
        }

        private void Update()
        {
            if (updateMode is UpdateMode.Update) LookAt();
        }

        private void LateUpdate()
        {
            if (updateMode is UpdateMode.LateUpdate) LookAt();
        }

        private void LookAt() => transform.LookAt(transform.position + CinemachineManager.MainCamTransform.rotation * Vector3.forward, CinemachineManager.MainCamTransform.rotation * Vector3.up);
    }
}