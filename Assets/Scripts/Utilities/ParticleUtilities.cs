using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace CanTemplate.Utils
{
    public static class ParticleUtilities
    {
        public static void PlayParticle(GameObject expObj, Transform objTransform, Vector3? pos = null, Vector3? scale = null, bool doParent = false)
        {
            var particle = GetParticle(objTransform, pos, scale, doParent, expObj);

            particle.Play();

            var main = particle.main;
            main.stopAction = ParticleSystemStopAction.Destroy;
        }

        public static void PlayParticle(string poolerName, Transform objTransform, Vector3? pos = null, Vector3? scale = null, bool doParent = false)
        {
            GameObject expObj = PoolerHandler.ReturnPooler(poolerName).GetObject();

            ParticleSystem particle = GetParticle(objTransform, pos, scale, doParent, expObj);

            particle.Play();
            var main = particle.main;
            float maxLifeTime = main.startLifetime.constantMax;
            var subEmitters = particle.GetComponentsInChildren<ParticleSystem>().Where(x => x != particle).ToList();
            if (subEmitters.Count > 0)
                maxLifeTime = subEmitters.Max(x => x.main.startLifetime.constantMax);

            DOVirtual.DelayedCall(main.duration + maxLifeTime + 2.5f, () =>
            {
                particle.Stop();
                expObj.GetComponent<Poolable>().ClearMe();
            });
        }

        private static ParticleSystem GetParticle(Transform objTransform, Vector3? pos, Vector3? scale, bool doParent, GameObject expObj)
        {
            var playPos = pos.HasValue ? pos.Value : doParent ? Vector3.zero : objTransform.position;
            var playScale = scale.HasValue ? scale.Value : expObj.transform.localScale;

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
