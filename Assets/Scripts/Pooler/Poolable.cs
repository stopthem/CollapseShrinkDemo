using UnityEngine;

namespace CanTemplate.Pooling
{
    public class Poolable : MonoBehaviour
    {
        public Pooler Pooler { get; private set; }

        public void ReturnToPool() => Pooler.Pool.Release(this);

        public void ResetParent() => Pooler.ResetParent(this);

        public void Init(Pooler pooler)
        {
            Pooler = pooler;
        }
    }
}