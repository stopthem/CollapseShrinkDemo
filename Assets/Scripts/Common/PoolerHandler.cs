using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PoolerHandler : MonoBehaviour
{
    private static PoolerHandler _instance;

    private Pooler[] _poolers;

    private void Awake()
    {
        _instance = this;
        _poolers = GetComponentsInChildren<Pooler>();
    }

    public static Pooler ReturnPooler(string name)
    {
        Pooler pooler = _instance._poolers.FirstOrDefault(x => x.name == name);
        if (!pooler)
        {
            Debug.Log("poolerhandler > couldn't find " + "<color=red>" + name + "</color>");
            return null;
        }

        return pooler;
    }
}