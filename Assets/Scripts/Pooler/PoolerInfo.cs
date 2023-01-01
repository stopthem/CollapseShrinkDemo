using System;
using NaughtyAttributes;
#if UNITY_EDITOR
using NaughtyAttributes.Editor;
#endif
using UnityEditor;
using UnityEngine;

namespace CanTemplate.Pooling
{
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
    public class PoolerInfoWithTag
    {
        public string poolerTag;

        [InfoBox("Init this in code"), ReadOnly, AllowNesting]
        public Pooler pooler;

        public void SetPooler() => pooler = PoolerHandler.GetPooler(poolerTag);
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