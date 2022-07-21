using System;
using System.Collections;
using System.Collections.Generic;
using DG.DeInspektor.Attributes;
using UnityEngine;
using UnityEngine.Pool;

public class Pooler : MonoBehaviour
{
    public enum PoolType
    {
        Stack,
        LinkedList
    }

    public PoolerInfo poolerInfo;

    private IObjectPool<Poolable> _pool;

    private IObjectPool<Poolable> Pool
    {
        get
        {
            if (_pool != null) return _pool;

            if (poolerInfo.poolType == PoolType.Stack)
                _pool = new ObjectPool<Poolable>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, poolerInfo.collectionChecks, 10, poolerInfo.maxPoolSize);
            else
                _pool = new LinkedPool<Poolable>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, poolerInfo.collectionChecks, poolerInfo.maxPoolSize);

            return _pool;
        }
    }

    private Transform PoolableParent => poolerInfo.makeChildOf ? poolerInfo.makeChildOf : transform;

    private void Start()
    {
        for (int i = 0; i < poolerInfo.spawnAtStart; i++)
        {
            var poolable = CreatePooledItem();
            poolable.ReturnToPool();
        }

        PoolerHandler.AddToPoolers(this);
    }

    private Poolable CreatePooledItem()
    {
        var item = Instantiate(poolerInfo.objectToPool, transform.position, poolerInfo.objectToPool.transform.rotation);

        var itemPoolable = item.GetComponent<Poolable>();

        if (itemPoolable) return itemPoolable;

        var type = item.GetComponent<ParticleSystem>() ? typeof(ParticlePoolable) : typeof(Poolable);
        itemPoolable = (Poolable)item.AddComponent(type);
        itemPoolable.Init(Pool);

        item.transform.SetParent(PoolableParent);

        return itemPoolable;
    }

    private void OnReturnedToPool(Poolable poolable)
    {
        var poolableTransform = poolable.transform;

        poolableTransform.SetParent(PoolableParent);

        poolableTransform.localScale = poolerInfo.objectToPool.transform.localScale;
        poolableTransform.rotation = poolerInfo.objectToPool.transform.rotation;
        poolableTransform.localPosition = Vector3.zero;

        poolable.gameObject.SetActive(false);
    }

    private void OnTakeFromPool(Poolable poolable)
    {
        poolable.gameObject.SetActive(true);
    }

    void OnDestroyPoolObject(Poolable poolable)
    {
        Destroy(poolable.gameObject);
    }

    public Poolable Get() => Pool.Get();
    public T Get<T>() => Pool.Get().GetComponent<T>();
}

[Serializable]
public class PoolerInfo
{
    public Pooler.PoolType poolType;

    [Space(5)] public GameObject objectToPool;

    public string poolerTag;

    public Transform makeChildOf;

    [Space(5), Tooltip("Will throw errors if we try to release an item that is already in the pool.")]
    public bool collectionChecks;

    public int maxPoolSize = 100;
    public int spawnAtStart;
}

[Serializable]
public class PooledObjInfo
{
    public float scaleMultiplier;
}

[Serializable]
public class PooledObjInfoWithTag:PooledObjInfo
{
    public string poolerTag;
}

[Serializable]
public class PooledObjInfoWithPooler : PooledObjInfo
{
    public Pooler pooler;
}