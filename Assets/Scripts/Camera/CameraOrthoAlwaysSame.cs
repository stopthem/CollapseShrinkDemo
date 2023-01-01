using Cinemachine;
using UnityEngine;

namespace CanTemplate.Camera
{
    [ExecuteInEditMode]
    public class CameraOrthoAlwaysSame : MonoBehaviour
    {
        [SerializeField] private float orthoSize;

        private void Start()
        {
            UpdateOrthoSize();
        }

#if UNITY_EDITOR
        private void Update()
        {
            UpdateOrthoSize();
        }
#endif

        private void UpdateOrthoSize()
        {
            var cam = CinemachineManager.CurrentCinemachineInfo?.cinemachineVirtualCamera;

#if UNITY_EDITOR
            cam = GetComponent<CinemachineVirtualCamera>();
#endif
            cam.m_Lens.OrthographicSize = orthoSize * Screen.height / Screen.width * 0.5f;
        }
    }
}