using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poolable : MonoBehaviour
{
    public bool imTaken { get; private set; }
    [HideInInspector] public Pooler myPooler;

    public void ClearMe() => myPooler.ClearObject(this);

    public void Taken(bool status) => imTaken = status;
}
