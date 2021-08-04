using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pooler : MonoBehaviour
{
    [SerializeField] private Transform makeChildOf;
    [SerializeField] private GameObject objectToPool;
    [SerializeField] private bool poolingCanvasObject;

    private List<GameObject> pooledObjects = new List<GameObject>();

    public List<GameObject> GetObjects(int amount)
    {
        List<GameObject> list = pooledObjects.Where(x => !x.GetComponent<Poolable>().imTaken).Take(amount).ToList();
        foreach (var item in list)
        {
            item.SetActive(true);
            item.GetComponent<Poolable>().imTaken = true;
        }

        if (list.Count < amount)
        {
            float value = amount - list.Count;
            for (int i = 0; i < value; i++)
            {
                GameObject item = Instantiate(objectToPool, transform.position, Quaternion.identity);

                if (poolingCanvasObject)
                {
                    item.transform.SetParent(makeChildOf);
                }
                else if (makeChildOf)
                {
                    item.transform.parent = makeChildOf;
                }
                else
                {
                    item.transform.parent = transform;
                }

                item.GetComponent<Poolable>().imTaken = true;
                pooledObjects.Add(item);
                list.Add(item);
            }
        }

        return list;
    }

    public void ClearObjects()
    {
        foreach (var poolable in pooledObjects)
        {
            poolable.GetComponent<Poolable>().imTaken = false;
            poolable.SetActive(false);
        }
    }

    public GameObject GetObject()
    {
        GameObject item;
        item = pooledObjects.Where(x => !x.GetComponent<Poolable>().imTaken).FirstOrDefault();
        if (!item)
        {
            item = Instantiate(objectToPool, transform.position, Quaternion.identity);
        }
        if (poolingCanvasObject)
        {
            item.transform.SetParent(makeChildOf);
        }
        else if (makeChildOf)
        {
            item.transform.parent = makeChildOf;
        }
        else
        {
            item.transform.parent = transform;
        }
        item.GetComponent<Poolable>().imTaken = true;
        pooledObjects.Add(item);
        return item;
    }
}
