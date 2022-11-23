using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace CanTemplate.Pooling
{
    public class Poolable : MonoBehaviour
    {
        public Pooler Pooler { get; private set; }

        public void ReturnToPool() => Pooler.Pool.Release(this);

        public void ResetParent() => transform.parent = Pooler.transform;

        public void Init(Pooler pooler)
        {
            Pooler = pooler;
        }
    }
}