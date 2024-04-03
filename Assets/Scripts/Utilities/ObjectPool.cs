using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : Singleton<ObjectPool>
{
    public List<Pool> pools = new List<Pool>();
    private Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();
    Transform blocks;

    public GameObject cubePrefab;
    public GameObject tntPrefab;
    public GameObject particlePrefab;

    private CubeFactory cubeFactory;

    protected override void Awake()
    {
        base.Awake();
        pools = new List<Pool>
        {
            new Pool { tag = "Cube", prefab = cubePrefab, size = 30 },
            new Pool { tag = "TNT", prefab = tntPrefab, size = 10 },
            new Pool { tag = "Particle", prefab = particlePrefab, size = 40}
        };


        PopulatePools();
        blocks = GameObject.Find("Blocks").transform;
        cubeFactory = GameObject.Find("BlockFactory").GetComponent<CubeFactory>();
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
            
            if (tag == "Cube")
            {
                return cubeFactory.CreateBlock("rand", position).GetComponent<GameObject>();
            } else if (tag == "Particle")
            {
                return Instantiate(particlePrefab, position, rotation);
            }
        }

        var objToSpawn = poolDictionary[tag].Dequeue();
        objToSpawn.transform.position = position;
        objToSpawn.transform.rotation = rotation;
        objToSpawn.transform.SetParent(objToSpawn.GetComponent<RectTransform>() != null ? null : blocks);
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

        if (tag == "Cube")
        {
            objectToReturn.GetComponent<Cube>().SetNormal();
        }

        objectToReturn.SetActive(false);
        objectToReturn.transform.SetParent(objectToReturn.GetComponent<RectTransform>() != null ? null : transform);
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
