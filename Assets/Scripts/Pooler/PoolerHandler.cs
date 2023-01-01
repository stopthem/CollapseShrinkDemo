using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CanTemplate.Pooling
{
    public class PoolerHandler : Singleton<PoolerHandler>
    {
        private List<Pooler> _poolers = new();

        public static void AddToPoolers(Pooler pooler)
        {
            if (Instance._poolers.FirstOrDefault(listPooler => listPooler.poolerInfo.poolerTag == pooler.poolerInfo.poolerTag))
            {
                return;
            }

            Instance._poolers.Add(pooler);
            Instance._poolers.RemoveAll(listPooler => !listPooler);
        }

        public static Pooler GetPooler(string tag, bool warnMsj = true)
        {
            var pooler = Instance._poolers.FirstOrDefault(x => x.poolerInfo.poolerTag == tag);
            if (pooler) return pooler;

            if (warnMsj) Debug.Log("PoolerHandler > couldn't find Pooler With Tag: " + "<color=red>" + tag + "</color>");
            return null;
        }

        public static Pooler CreatePooler(PoolerInfo poolerInfo)
        {
            var foundPooler = GetPooler(poolerInfo.poolerTag, false);
            if (foundPooler) return foundPooler;

            var pooler = new GameObject($"{poolerInfo.poolerTag} Pooler")
            {
                transform =
                {
                    parent = Instance.transform,
                    localPosition = Vector3.zero
                }
            };

            var poolerScript = pooler.AddComponent<Pooler>();
            poolerScript.poolerInfo = poolerInfo;
            AddToPoolers(poolerScript);
            return poolerScript;
        }
    }
}