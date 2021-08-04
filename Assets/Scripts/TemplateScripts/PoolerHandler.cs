using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PoolerHandler : MonoBehaviour
{
    private static PoolerHandler instance;

    private Pooler[] poolers;

    private void Awake()
    {
        instance = this;
        poolers = GetComponentsInChildren<Pooler>();
    }

    public static Pooler ReturnPooler(string name)
    {
        return instance.poolers.First(x => x.name == name);
    }
}
