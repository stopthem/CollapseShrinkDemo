using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PoolerHandler : Singleton<PoolerHandler>
{
    private Pooler[] _poolers;

    private void Awake()
    {
        _poolers = GetComponentsInChildren<Pooler>();
    }

    public static Pooler ReturnPooler(string name)
    {
        Pooler pooler = Instance._poolers.FirstOrDefault(x => x.name == name);
        if (!pooler)
        {
            Debug.Log("poolerhandler > couldn't find " + "<color=red>" + name + "</color>");
            return null;
        }
        return pooler;
    }
}