using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CanTemplate.Pooling;
using DG.Tweening;
using UnityEngine;

namespace CanTemplate.Utilities
{
    public static class ParticleUtilities
    {
        /// <summary>
        /// Plays the particle from given object.
        /// </summary>
        /// <param name="particleThe particle system</param>
        /// <param name="stopAction">If null, given particleSystem's stopAction will be used</param>
        /// <returns></returns>
        public static ParticleSystem PlayParticle(ParticleSystem particle, ParticlePlayOptions particlePlayOptions, ParticleSystemStopAction? stopAction = null)
        {
            SetParticle(particlePlayOptions, particle);

            particle.Play();

            var main = particle.main;
            main.stopAction = stopAction ?? main.stopAction;

            return particle;
        }

        /// <summary>
        /// Plays the particle from found pooler which is found by given poolerTag and returns it to the pooler after playing.
        /// </summary>
        /// <param name="poolerTag"></param>
        /// <returns></returns>
        public static ParticleSystem PlayParticle(string poolerTag, ParticlePlayOptions particlePlayOptions)
        {
            var particle = PoolerHandler.GetPooler(poolerTag)?.Get<ParticleSystem>();

            if (!particle)
                return null;

            SetParticle(particlePlayOptions, particle);

            particle.Play();

            return particle;
        }

        /// <summary>
        /// Plays the particle from found pooler which is found by given <see cref="PooledObjInfoWithTag"/> and returns it to the pooler after playing.
        /// </summary>
        /// <param name="pooledObjInfoWithTag"></param>
        /// <returns></returns>
        public static ParticleSystem PlayParticle(PooledObjInfoWithTag pooledObjInfoWithTag, ParticlePlayOptions particlePlayOptions)
        {
            var particle = PoolerHandler.GetPooler(pooledObjInfoWithTag.poolerTag)?.Get<ParticleSystem>();

            if (!particle)
                return null;

            particlePlayOptions.playScale = Vector3.one * pooledObjInfoWithTag.scaleMultiplier;
            SetParticle(particlePlayOptions, particle);

            particle.Play();

            return particle;
        }

        /// <summary>
        /// Plays the particle from found pooler which is found by given <see cref="PooledObjInfoWithPooler"/> and returns it to the pooler after playing.
        /// </summary>
        /// <param name="pooledObjInfoWithPooler"></param>
        /// <returns></returns>
        public static ParticleSystem PlayParticle(PooledObjInfoWithPooler pooledObjInfoWithPooler, ParticlePlayOptions particlePlayOptions)
        {
            var particle = pooledObjInfoWithPooler.pooler.Get<ParticleSystem>();

            if (!particle)
                return null;

            particlePlayOptions.playScale = Vector3.one * pooledObjInfoWithPooler.scaleMultiplier;
            SetParticle(particlePlayOptions, particle);

            particle.Play();

            return particle;
        }


        /// <summary>
        /// Plays the particle from given pooler and returns it to the pooler after playing.
        /// </summary>
        /// <param name="pooler"></param>
        /// <returns></returns>
        public static ParticleSystem PlayParticle(Pooler pooler, ParticlePlayOptions particlePlayOptions)
        {
            var particle = pooler.Get<ParticleSystem>();

            SetParticle(particlePlayOptions, particle);

            particle.Play();

            return particle;
        }

        private static ParticleSystem SetParticle(ParticlePlayOptions particlePlayOptions, ParticleSystem particle)
        {
            particle.gameObject.SetActive(true);

            var particleTransform = particle.transform;

            particlePlayOptions.playPos ??= particlePlayOptions.doParent ? Vector3.zero : particlePlayOptions.PlayTransform.position;
            particlePlayOptions.playScale ??= particleTransform.localScale;
            particlePlayOptions.playRotation ??= particleTransform.rotation;

            if (particlePlayOptions.doParent)
            {
                particleTransform.parent = particlePlayOptions.PlayTransform;
                particleTransform.localPosition = particlePlayOptions.playPos.Value;
                particleTransform.localRotation = particlePlayOptions.playRotation.Value;
            }
            else
            {
                particleTransform.position = particlePlayOptions.playPos.Value;
                particleTransform.rotation = particlePlayOptions.playRotation.Value;
            }

            particleTransform.localScale = particlePlayOptions.playScale.Value;

            if (particlePlayOptions.color.HasValue)
            {
                ChangeParticleColor(particle, particlePlayOptions.color.Value);
            }

            return particle;
        }

        /// <summary>
        /// Only changes start color of given particle and its particle childs.
        /// </summary>
        /// <param name="particle"></param>
        /// <param name="color"></param>
        public static void ChangeParticleColor(ParticleSystem particle, Color color, bool includeInactive = true)
        {
            foreach (var particleSystem in particle.GetComponentsInChildren<ParticleSystem>(includeInactive))
            {
                var main = particleSystem.main;
                main.startColor = color;
            }
        }

        public class ParticlePlayOptions
        {
            public ParticlePlayOptions(Transform playTransform)
            {
                PlayTransform = playTransform;
            }

            public Transform PlayTransform { get; }

            /// <summary>
            /// Can be null
            /// <para>if non is given, particle will play with a position of playTransform.position</para>
            /// </summary>
            public Vector3? playPos;

            /// <summary>
            /// Can be null
            /// <para>if non is given, particle will play with a scale of Vector3.one</para>
            /// </summary>
            public Vector3? playScale;

            /// <summary>
            /// Only going to change start colors if has a value
            /// </summary>
            public Color? color;

            public Quaternion? playRotation;
            public bool doParent = false;
        }
    }
}