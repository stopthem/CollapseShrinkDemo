using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace CanTemplate.Utilities
{
    public static class ParticleUtilities
    {
        /// <summary>
        /// Plays the particle from given object.
        /// </summary>
        /// <param name="expObj">The particle system</param>
        /// <param name="objTransform"></param>
        /// <param name="pos">If setParent is true, note that this will be used as localPosition.</param>
        /// <param name="scale"></param>
        /// <param name="doParent"></param>
        /// <param name="stopAction">If null, given particleSystem's stopAction will be used</param>
        /// <returns></returns>
        public static ParticleSystem PlayParticle(GameObject expObj, Transform objTransform, Vector3? pos = null,
            Vector3? scale = null, bool doParent = false, ParticleSystemStopAction? stopAction = null)
        {
            var particle = SetParticle(objTransform, pos, scale, doParent, expObj.GetComponent<ParticleSystem>());

            particle.Play();

            var main = particle.main;
            main.stopAction = stopAction ?? main.stopAction;

            return particle;
        }

        /// <summary>
        /// Plays the particle from found pooler which is found by given poolerName and returns it to the pooler after playing.
        /// </summary>
        /// <param name="poolerTag"></param>
        /// <param name="objTransform"></param>
        /// <param name="pos">If setParent is true, note that this will be used as localPosition.</param>
        /// <param name="scale"></param>
        /// <param name="doParent"></param>
        /// <returns></returns>
        public static ParticleSystem PlayParticle(string poolerTag, Transform objTransform, Vector3? pos = null,
            Vector3? scale = null, bool doParent = false)
        {
            var particle = PoolerHandler.GetPooler(poolerTag)?.Get<ParticleSystem>();

            if (!particle)
                return null;

            SetParticle(objTransform, pos, scale, doParent, particle);

            particle.Play();

            return particle;
        }

        /// <summary>
        /// Plays the particle from found pooler which is found by given pooledParticleInfo and returns it to the pooler after playing.
        /// </summary>
        /// <param name="pooledObjInfoWithTag"></param>
        /// <param name="objTransform"></param>
        /// <param name="pos">If setParent is true, note that this will be used as localPosition.</param>
        /// <param name="scale"></param>
        /// <param name="doParent"></param>
        /// <returns></returns>
        public static ParticleSystem PlayParticle(PooledObjInfoWithTag pooledObjInfoWithTag, Transform objTransform, Vector3? pos = null,
            Vector3? scale = null, bool doParent = false)
        {
            var particle = PoolerHandler.GetPooler(pooledObjInfoWithTag.poolerTag)?.Get<ParticleSystem>();

            if (!particle)
                return null;

            SetParticle(objTransform, pos, scale ?? Vector3.one * pooledObjInfoWithTag.scaleMultiplier, doParent, particle);

            particle.Play();

            return particle;
        }


        /// <summary>
        /// Plays the particle from given pooler and returns it to the pooler after playing.
        /// </summary>
        /// <param name="pooler"></param>
        /// <param name="objTransform"></param>
        /// <param name="pos">If setParent is true, note that this will be used as localPosition.</param>
        /// <param name="scale"></param>
        /// <param name="doParent"></param>
        /// <returns></returns>
        public static ParticleSystem PlayParticle(Pooler pooler, Transform objTransform, Vector3? pos = null,
            Vector3? scale = null, bool doParent = false)
        {
            var particle = pooler.Get<ParticleSystem>();

            SetParticle(objTransform, pos, scale, doParent, particle);

            particle.Play();

            return particle;
        }

        private static ParticleSystem SetParticle(Transform objTransform, Vector3? pos, Vector3? scale, bool doParent,
            ParticleSystem particle)
        {
            var playPos = pos ?? (doParent ? Vector3.zero : objTransform.position);
            var playScale = scale ?? particle.transform.localScale;

            if (doParent)
            {
                particle.transform.parent = objTransform;
                particle.transform.localPosition = playPos;
            }
            else particle.transform.position = playPos;

            if (playScale != Vector3.zero)
                particle.transform.localScale = playScale;

            return particle;
        }
    }
}