using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : MonoBehaviour
{
    Queue<T> pool;
    T prefab;

    public ObjectPool(T prefab, int initalSize, Transform parent)
    {
        this.prefab = prefab;
        pool = new Queue<T>();

        for(int i=0; i<initalSize; i++)
        {
            T instce = Object.Instantiate(prefab, parent);
            instce.gameObject.SetActive(false);
            pool.Enqueue(instce);
        }
    }

    public T GetObject()
    {
        Debug.Log("Ç® °³¼ö : " + pool.Count);
        if (pool.Count > 0)
        {
            T obj = pool.Dequeue();
            obj.gameObject.SetActive(true);
            return obj;
        }

        T newObj = Object.Instantiate(prefab);
        return newObj;
    }

    public void ReturnObject(T obj)
    {
        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
    }

}
