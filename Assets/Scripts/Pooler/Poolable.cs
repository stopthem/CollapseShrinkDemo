using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Poolable : MonoBehaviour
{
    public IObjectPool<Poolable> Pool { get; private set; }

    public void ReturnToPool() => Pool.Release(this);

    public void Init(IObjectPool<Poolable> pooler)
    {
        Pool = pooler;
    }
}