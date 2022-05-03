using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager instance;

    public List<ObjectPool> objectPoolList;
    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(instance);

        for (int i = 0; i < objectPoolList.Count; i++)
        {
            CreateObjectPool(objectPoolList[i]);
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    void CreateObjectPool(ObjectPool objectPool)
    {
        for (int i = 0; i < objectPool.pooledAmount; i++)
        {
            objectPool.pooledObjects = new List<GameObject>();

            GameObject obj = GetItem(objectPool.poolObject);
            obj.SetActive(false);
            objectPool.pooledObjects.Add(obj);
        }
    }

    public GameObject GetItem(string key)
    {
        for (int i = 0; i < objectPoolList.Count; i++)
        {
            if (objectPoolList[i].key == key)
            {
                //objectPoolList[i].GetPooledObject()
                return objectPoolList[i].GetPooledObject();
            }
        }

        return null;
    }

    public GameObject GetItem(GameObject original)
    {
        for (int i = 0; i < objectPoolList.Count; i++)
        {
            if (objectPoolList[i].poolObject == original)
            {
                return objectPoolList[i].GetPooledObject();
            }
        }

        //If no such pool for given object make a growing pool
        ObjectPool objectPool = new ObjectPool(original, 5, true, original.name);
        objectPoolList.Add(objectPool);
        CreateObjectPool(objectPool);

        return objectPool.GetPooledObject();
    }

    //Variant that allows to set transform
    public GameObject GetItem(GameObject original, Vector3 position)
    {
        for (int i = 0; i < objectPoolList.Count; i++)
        {
            if (objectPoolList[i].poolObject == original)
            {
                GameObject obj = objectPoolList[i].GetPooledObject();
                obj.transform.position = position;
                return obj;
            }
        }

        //If no such pool for given object make a growing pool
        ObjectPool objectPool = new ObjectPool(original, 1, true, original.name);
        objectPoolList.Add(objectPool);
        CreateObjectPool(objectPool);

        GameObject obj_2 = objectPool.GetPooledObject();
        obj_2.transform.position = position;
        return obj_2;
    }

    //Variant with transform and rotation
    public GameObject GetItem(GameObject original, Vector3 position, Quaternion rotation)
    {
        for (int i = 0; i < objectPoolList.Count; i++)
        {
            if (objectPoolList[i].poolObject == original)
            {
                GameObject obj = objectPoolList[i].GetPooledObject();
                obj.transform.position = position;
                obj.transform.localRotation = rotation;
                return obj;
            }
        }

        //If no such pool for given object make a growing pool
        ObjectPool objectPool = new ObjectPool(original, 5, true, original.name);
        objectPoolList.Add(objectPool);
        CreateObjectPool(objectPool);
        GameObject obj_2 = objectPool.GetPooledObject();
        obj_2.transform.position = position;
        obj_2.transform.rotation = rotation;
        return obj_2;
    }

    //Destroy functions to clear objects without actually destroying them.
    public void ClearObject(GameObject obj)
    {
        obj.SetActive(false);
    }
    public void ClearObject(GameObject obj, float t)
    {
        StartCoroutine(Clear(obj, t));
    }
    IEnumerator Clear(GameObject obj, float t)
    {
        yield return new WaitForSeconds(t);
        obj.SetActive(false);
    }
}

[System.Serializable]
public class ObjectPool
{
    public GameObject poolObject;
    public int pooledAmount;

    public string key;

    public bool willGrow;

    public List<GameObject> pooledObjects;

    public ObjectPool(GameObject gameObject, int amount)
    {
        poolObject = gameObject;
        pooledAmount = amount;
        key = gameObject.name;
    }
    public ObjectPool(GameObject gameObject, int amount, bool grow)
    {
        poolObject = gameObject;
        pooledAmount = amount;
        willGrow = grow;
        key = gameObject.name;
    }

    public ObjectPool(GameObject gameObject, int amount, bool grow, string name)
    {
        poolObject = gameObject;
        pooledAmount = amount;
        willGrow = grow;
        key = gameObject.name;
        key = name;
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                pooledObjects[i].SetActive(true);
                return pooledObjects[i];
            }
        }

        if (willGrow)
        {
            GameObject obj = GameObject.Instantiate(poolObject);
            pooledObjects.Add(obj);
            return obj;
        }

        return null;
    }
}


