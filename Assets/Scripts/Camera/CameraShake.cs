using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

namespace CanTemplate.Camera
{
    public class CameraShake : MonoBehaviour
    {
        public static CameraShake instance;

        private Vector3 _originalPos;
        private float _timeAtCurrentFrame;
        private float _timeAtLastFrame;
        private float _fakeDelta;

        private bool _canShake = true;

        private void Awake()
        {
            instance = this;
        }

        private void Update()
        {
            // Calculate a fake delta time, so we can Shake while game is paused.
            _timeAtCurrentFrame = Time.realtimeSinceStartup;
            _fakeDelta = _timeAtCurrentFrame - _timeAtLastFrame;
            _timeAtLastFrame = _timeAtCurrentFrame;
        }

        public static void Shake(ShakeInfo shakeInfo)
        {
            if (instance._canShake)
            {
                instance._canShake = false;
                instance._originalPos = instance.gameObject.transform.localPosition;
                instance.StopAllCoroutines();
                instance.StartCoroutine(instance.ShakeRoutine(shakeInfo));
            }
        }

        private IEnumerator ShakeRoutine(ShakeInfo shakeInfo)
        {
            var duration = shakeInfo.duration;

            while (duration > 0)
            {
                transform.localPosition = _originalPos + Random.insideUnitSphere * shakeInfo.intensity;

                duration -= _fakeDelta;
                yield return null;
            }

            _canShake = true;
            transform.localPosition = _originalPos;
        }
    }

    [Serializable]
    public class ShakeInfo
    {
        public float intensity;
        public float duration;
        public float smoothFinishDuration;
    }
}