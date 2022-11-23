using System;
using System.Collections.Generic;
using System.Linq;
using CanTemplate.Extensions;
using CanTemplate.Pooling;
using NaughtyAttributes;
#if UNITY_EDITOR
using NaughtyAttributes.Editor;
#endif
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

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
            for (int i = 0; i < poolerInfo.spawnAtStart; i++)
            {
                var poolable = CreatePooledItem();
                poolable.ReturnToPool();
            }

            PoolerHandler.AddToPoolers(this);
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

        [Button(enabledMode: EButtonEnableMode.Editor)]
        public void SetPoolerTagAndObjNameAuto()
        {
            if (!poolerInfo.objectToPool) return;

            poolerInfo.poolerTag = poolerInfo.objectToPool.name;
            name = $"{poolerInfo.objectToPool.name}Pooler";
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

    [Serializable]
    public class PoolerInfo
    {
        public PoolerInfo(GameObject objectToPool)
        {
            this.objectToPool = objectToPool;
            poolerTag = objectToPool.name;
        }

        public PoolerInfo(PoolerInfo poolerInfoToCopy)
        {
            poolType = poolerInfoToCopy.poolType;
            objectToPool = poolerInfoToCopy.objectToPool;
            poolerTag = poolerInfoToCopy.poolerTag;
            makeChildOf = poolerInfoToCopy.makeChildOf;
            collectionChecks = poolerInfoToCopy.collectionChecks;
            maxPoolSize = poolerInfoToCopy.maxPoolSize;
            spawnAtStart = poolerInfoToCopy.spawnAtStart;
        }

        public Pooler.PoolType poolType;

        public bool poolMultiple;

        [ShowIf("poolMultiple"), AllowNesting, InfoBox("Pfs To Pool cannot be empty", EInfoBoxType.Warning)]
        public GameObject[] pfsToPool;

        [Space(5), HideIf("poolMultiple"), Required("Object To Pool cannot be null"), AllowNesting]
        public GameObject objectToPool;

        [AllowNesting, ValidateInput("IsNullOrEmpty", "Pooler Tag cannot be empty"),]
        public string poolerTag;

        public Transform makeChildOf;

        [Space(5), Tooltip("Will throw errors if we try to release an item that is already in the pool.")]
        public bool collectionChecks;

        [AllowNesting, ValidateInput("IsGreaterThanZero", "Max Pool Size must be greater than zero(recommended 1000)")]
        public int maxPoolSize = 1000;

        public int spawnAtStart;

        private bool IsGreaterThanZero(int value)
        {
            return value > 0;
        }

        private bool IsNullOrEmpty(string s) => !string.IsNullOrEmpty(s);

        public void SetPoolerTagAuto() => poolerTag = objectToPool.name;
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(PoolerInfo))]
    public class PoolerInfoDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var childDepth = property.depth + 1;
            EditorGUILayout.PropertyField(property, label, false);
            if (!property.isExpanded)
            {
                return;
            }

            EditorGUI.indentLevel++;
            foreach (SerializedProperty child in property)
            {
                if (child.depth == childDepth && PropertyUtility.IsVisible(child))
                {
                    EditorGUILayout.PropertyField(child);
                }
            }

            EditorGUI.indentLevel--;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, includeChildren: false)
                   - EditorGUIUtility.singleLineHeight - EditorGUIUtility.standardVerticalSpacing;
        }
    }
#endif

    [Serializable]
    public class PooledObjInfo
    {
        public float scaleMultiplier = 1;
    }

    [Serializable]
    public class PooledObjInfoWithTag : PooledObjInfo

    {
        public string poolerTag;
    }

    [Serializable]
    public class PooledObjInfoWithPooler : PooledObjInfo
    {
        [InfoBox("If this object is not a child of a DontDestroyOnLoad object and 'Pooler' is a child of Hierarchy>Poolers(or any DontDestroyOnLoad object) this will return an error once scene is loaded-reloaded", EInfoBoxType.Error)]
        public Pooler pooler;
    }

    [Serializable]
    public class PooledObjInfoWithPoolerInfo : PooledObjInfo
    {
        public PoolerInfo poolerInfo;
    }
}