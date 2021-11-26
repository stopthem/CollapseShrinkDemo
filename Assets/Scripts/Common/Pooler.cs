using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pooler : MonoBehaviour
{
    [SerializeField] private Transform makeChildOf;
    public GameObject objectToPool;
    [SerializeField] private int spawnAtAwake;

    private List<Poolable> pooledObjects = new List<Poolable>();

    private void Awake()
    {
        if (spawnAtAwake > 0) pooledObjects = GetObjects(spawnAtAwake).Select(x => x.GetComponent<Poolable>()).ToList();
        ClearObjects();
    }
    public List<GameObject> GetObjects(int amount, bool worldPositionStays = true, bool selectRandom = false)
    {
        List<GameObject> list = pooledObjects.Where(x => !x.imTaken).Select(x => x.gameObject).ToList();

        if (selectRandom)
            list = list.OrderBy(x => Random.Range(0, list.Count)).Take(amount).ToList();
        else
            list = list.Take(amount).ToList();

        if (list.Count < amount)
        {
            float value = amount - list.Count;
            for (int i = 0; i < value; i++)
            {
                GameObject item = Instantiate(objectToPool, transform.position, objectToPool.transform.rotation);

                Transform makeChild = makeChildOf ? makeChildOf : transform;
                item.transform.SetParent(makeChild, worldPositionStays);

                Poolable itemPoolable = item.GetComponent<Poolable>();
                if (!itemPoolable) itemPoolable = item.AddComponent<Poolable>();

                pooledObjects.Add(itemPoolable);
                list.Add(item);
            }
        }

        foreach (var item in list)
        {
            item.SetActive(true);
            Poolable itemPoolable = item.GetComponent<Poolable>();

            if (!itemPoolable) itemPoolable = item.AddComponent<Poolable>();

            itemPoolable.Taken(true);
            itemPoolable.myPooler = this;
        }

        return list;
    }

    public void ClearObjects()
    {
        foreach (var poolable in pooledObjects)
        {
            poolable.Taken(false);
            poolable.gameObject.SetActive(false);
        }
    }

    public void ClearObject(Poolable poolable)
    {
        if (poolable.GetComponent<RectTransform>())
            poolable.transform.SetParent(transform);
        else
            poolable.transform.parent = transform;

        poolable.transform.localScale = objectToPool.transform.localScale;
        poolable.transform.rotation = objectToPool.transform.rotation;
        poolable.transform.localPosition = Vector3.zero;

        poolable.gameObject.SetActive(false);
        poolable.Taken(false);
    }

    public GameObject GetObject(bool worldPositionStays = true, bool selectRandom = false) => GetObjects(1, worldPositionStays, selectRandom)[0];
}
