using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CanTemplate.Pooling
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticlePoolable : Poolable
    {
        private ParticleSystem _particleSystem;

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();

            var main = _particleSystem.main;
            main.stopAction = ParticleSystemStopAction.Callback;
        }

        private void OnParticleSystemStopped() => ReturnToPool();
    }
}