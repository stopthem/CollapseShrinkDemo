using System.Collections.Generic;
using System.Linq;
using CanTemplate.Extensions;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace CanTemplate.Pooling
{
    public class Pooler : MonoBehaviour
    {
        public enum PoolType
        {
            Stack,
            LinkedList
        }

        public PoolerInfo poolerInfo;

        private IObjectPool<Poolable> _pool;

        public IObjectPool<Poolable> Pool
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

        public List<Poolable> PooledObjects { get; } = new();

        private Transform PoolableParent => poolerInfo.makeChildOf ? poolerInfo.makeChildOf : transform;

        private void Start()
        {
            if (transform.root.GetComponent<PoolerHandler>())
            {
                SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
            }

            for (int i = 0; i < poolerInfo.spawnAtStart; i++)
            {
                var poolable = CreatePooledItem();
                poolable.ReturnToPool();
            }

            PoolerHandler.AddToPoolers(this);
        }

        private void OnDestroy()
        {
            SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
        }

        private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene arg1)
        {
            if (!transform.root.GetComponent<PoolerHandler>())
            {
                SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
                return;
            }

            ReturnAllPooled();
        }

        private void ReturnAllPooled()
        {
            foreach (var poolable in PooledObjects.Where(poolable => poolable))
            {
                poolable.ReturnToPool();
            }

            PooledObjects.Clear();
        }

        private Poolable CreatePooledItem()
        {
            var pf = poolerInfo.poolMultiple ? poolerInfo.pfsToPool.GetRandomElement() : poolerInfo.objectToPool;
            var item = Instantiate(pf, transform.position, pf.transform.rotation);

            var itemPoolable = item.GetComponent<Poolable>();

            if (!itemPoolable)
            {
                var type = item.GetComponent<ParticleSystem>() ? typeof(ParticlePoolable) : typeof(Poolable);
                itemPoolable = (Poolable)item.AddComponent(type);
            }

            itemPoolable.Init(this);
            item.transform.SetParent(PoolableParent);

            PooledObjects.Add(itemPoolable);

            return itemPoolable;
        }

        private void OnReturnedToPool(Poolable poolable)
        {
            var poolableTransform = poolable.transform;

            poolableTransform.SetParent(PoolableParent);

            if (poolerInfo.objectToPool)
            {
                poolableTransform.localScale = poolerInfo.objectToPool.transform.localScale;
                poolableTransform.rotation = poolerInfo.objectToPool.transform.rotation;
            }

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

        [Button("Set Pooler Tag, Obj Name, And Default Values", EButtonEnableMode.Editor)]
        public void SetPoolerTagAndObjNameAuto()
        {
            if (!poolerInfo.objectToPool) return;

            poolerInfo.poolerTag = poolerInfo.objectToPool.name;
            name = $"{poolerInfo.objectToPool.name}Pooler";

            poolerInfo.maxPoolSize = 1000;
        }

        public void ResetParent(Poolable poolable) => poolable.transform.parent = poolerInfo.makeChildOf ? poolerInfo.makeChildOf : transform;

        public Poolable Get() => Pool.Get();
        public T Get<T>() => Pool.Get().GetComponent<T>();

        public List<Poolable> GetMultiple(int count)
        {
            var pooledList = new List<Poolable>();

            for (int i = 0; i < count; i++)
            {
                pooledList.Add(Get());
            }

            return pooledList;
        }

        public List<T> GetMultiple<T>(int count)
        {
            var pooledList = new List<T>();

            for (int i = 0; i < count; i++)
            {
                pooledList.Add(Get().GetComponent<T>());
            }

            return pooledList;
        }
    }
}