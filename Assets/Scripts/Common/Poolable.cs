using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poolable : MonoBehaviour
{
    [HideInInspector] public bool imTaken;
    [HideInInspector] public Pooler myPooler;

    public void ClearMe() => myPooler.ClearObject(this);
}
