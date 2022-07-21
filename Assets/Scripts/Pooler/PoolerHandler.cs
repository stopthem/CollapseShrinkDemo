using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CanTemplate.Extensions;
using UnityEngine;
using UnityEngine.Pool;

public class PoolerHandler : MonoBehaviour
{
    public static PoolerHandler instance;

    private List<Pooler> _poolers = new();

    private void Awake()
    {
        instance = this;
    }

    public static void AddToPoolers(Pooler pooler) => instance._poolers.AddIfNotPresent(pooler);

    public static Pooler GetPooler(string tag)
    {
        var pooler = instance._poolers.FirstOrDefault(x => x.poolerInfo.poolerTag == tag);
        if (pooler) return pooler;

        Debug.Log("PoolerHandler > couldn't find Pooler With Tag: " + "<color=red>" + tag + "</color>");
        return null;
    }

    public static Pooler CreatePooler(PoolerInfo poolerInfo, string name)
    {
        var pooler = new GameObject(name)
        {
            transform =
            {
                parent = instance.transform,
                localPosition = Vector3.zero
            }
        };

        var poolerScript = pooler.AddComponent<Pooler>();
        poolerScript.poolerInfo = poolerInfo;
        return poolerScript;
    }
}