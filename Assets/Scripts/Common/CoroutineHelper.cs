using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineHelper : Singleton<CoroutineHelper>
{
    public static void StartStaticCoroutine(IEnumerator routine) => Instance.StartCoroutine(routine);
}