using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace CanTemplate.Utils
{
    public static class ParticleUtilities
    {
        /// <summary>
        /// Plays the particle from given object.
        /// </summary>
        /// <param name="objTransform"></param>
        /// <param name="pos">If setParent is true, note that this will be used as localPosition.</param>
        /// <param name="scale"></param>
        /// <param name="doParent"></param>
        /// <returns></returns>
        public static ParticleSystem PlayParticle(GameObject expObj, Transform objTransform, Vector3? pos = null,
            Vector3? scale = null, bool doParent = false, ParticleSystemStopAction stopAction = ParticleSystemStopAction.None)
        {
            var particle = GetParticle(objTransform, pos, scale, doParent, expObj);

            particle.Play();

            var main = particle.main;
            main.stopAction = stopAction;

            return particle;
        }

        /// <summary>
        /// Plays the particle from found pooler which is found by given poolerName and returns it to the pooler after playing.
        /// </summary>
        /// <param name="poolerName"></param>
        /// <param name="objTransform"></param>
        /// <param name="pos">If setParent is true, note that this will be used as localPosition.</param>
        /// <param name="scale"></param>
        /// <param name="doParent"></param>
        /// <returns></returns>
        public static ParticleSystem PlayParticle(string poolerName, Transform objTransform, Vector3? pos = null,
            Vector3? scale = null, bool doParent = false)
        {
            GameObject expObj = PoolerHandler.ReturnPooler(poolerName).GetObject();

            var particle = PlayPoolerParticle(objTransform, pos, scale, doParent, expObj);

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
            GameObject expObj = pooler.GetObject();

            var particle = PlayPoolerParticle(objTransform, pos, scale, doParent, expObj);

            return particle;
        }

        private static ParticleSystem PlayPoolerParticle(Transform transform, Vector3? vector3, Vector3? scale1, bool b, GameObject gameObject)
        {
            ParticleSystem particle = GetParticle(transform, vector3, scale1, b, gameObject);

            particle.Play();
            var main = particle.main;
            float maxLifeTime = main.startLifetime.constantMax;
            var subEmitters = particle.GetComponentsInChildren<ParticleSystem>().Where(x => x != particle).ToList();
            if (subEmitters.Count > 0)
                maxLifeTime = subEmitters.Max(x => x.main.startLifetime.constantMax);

            DOVirtual.DelayedCall(main.duration + maxLifeTime + 2.5f, () =>
            {
                particle.Stop();
                gameObject.GetComponent<Poolable>().ClearMe();
            }, false);
            return particle;
        }

        private static ParticleSystem GetParticle(Transform objTransform, Vector3? pos, Vector3? scale, bool doParent,
            GameObject expObj)
        {
            var playPos = pos ?? (doParent ? Vector3.zero : objTransform.position);
            var playScale = scale ?? expObj.transform.localScale;

            if (doParent) expObj.transform.parent = objTransform;

            var particle = expObj.GetComponent<ParticleSystem>();
            if (!doParent) expObj.transform.position = playPos;
            else expObj.transform.localPosition = playPos;

            if (playScale != Vector3.zero)
                expObj.transform.localScale = playScale;

            return particle;
        }
    }
}