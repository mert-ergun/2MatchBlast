using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPool : Singleton<ObjectPool>
{
    public List<Pool> pools = new List<Pool>();
    private Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();

    protected override void Awake()
    {
        base.Awake();
        PopulatePools();
    }

    private void PopulatePools()
    {
        foreach (var pool in pools)
        {
            var objectQueue = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                var obj = Instantiate(pool.prefab);
                obj.transform.SetParent(obj.GetComponent<RectTransform>() != null ? null : transform);
                obj.SetActive(false);
                objectQueue.Enqueue(obj);
            }

            poolDictionary[pool.tag] = objectQueue;
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogError($"No pool exists with tag: {tag}");
            return null;
        }

        if (poolDictionary[tag].Count == 0)
        {
            Debug.LogError($"Pool with tag: {tag} is empty!");
            return null;
        }

        var objToSpawn = poolDictionary[tag].Dequeue();
        objToSpawn.transform.position = position;
        objToSpawn.transform.rotation = rotation;
        objToSpawn.SetActive(true);

        return objToSpawn;
    }

    public void ReturnToPool(string tag, GameObject objectToReturn)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag: {tag} does not exist.");
            return;
        }

        objectToReturn.SetActive(false);
        poolDictionary[tag].Enqueue(objectToReturn);
    }

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }
}
