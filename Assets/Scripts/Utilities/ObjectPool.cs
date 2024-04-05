using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages pools of objects for efficient object reuse. This is a design pattern that improves performance by reusing objects instead of creating and destroying them.
/// Also is a Singleton class. This means that only one instance of the class can exist in the game at any time.
/// </summary>
public class ObjectPool : Singleton<ObjectPool>
{
    /// <summary>
    /// A list of all the pools managed by this object pool.
    /// </summary>
    public List<Pool> pools = new List<Pool>();

    /// <summary>
    /// A dictionary to quickly access the queue of objects for a specific tag.
    /// </summary>
    private Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();

    /// <summary>
    /// The parent transform under which the pooled objects are organized in the scene hierarchy.
    /// </summary>
    Transform blocks;

    // Prefabs for different types of objects to be pooled.
    public GameObject cubePrefab;
    public GameObject tntPrefab;
    public GameObject particlePrefab;

    // Factories for creating new instances when the pool is empty.
    private CubeFactory cubeFactory;
    private TNTFactory tntFactory;

    /// <summary>
    /// Initializes the object pool and populates the pools based on the predefined settings.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        pools = new List<Pool>
        {
            new Pool { tag = "Cube", prefab = cubePrefab, size = 30 },
            new Pool { tag = "TNT", prefab = tntPrefab, size = 10 },
            new Pool { tag = "Particle", prefab = particlePrefab, size = 100}
        };


        PopulatePools();
        blocks = GameObject.Find("Blocks").transform;
        cubeFactory = GameObject.Find("BlockFactory").GetComponent<CubeFactory>();
        tntFactory = GameObject.Find("BlockFactory").GetComponent<TNTFactory>();
    }

    /// <summary>
    /// Populates the pools with objects up to the specified size.
    /// </summary>
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

    /// <summary>
    /// Spawns an object from the pool or creates a new one if the pool is empty.
    /// </summary>
    /// <param name="tag">The tag identifying the pool.</param>
    /// <param name="position">The position where the object will be spawned.</param>
    /// <param name="rotation">The rotation of the spawned object.</param>
    /// <returns>The spawned GameObject.</returns>
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogError($"No pool exists with tag: {tag}");
            return null;
        }

        if (poolDictionary[tag].Count == 0)
        {
            if (tag == "Cube")
            {
                return cubeFactory.CreateBlock("rand", position);
            } else if (tag == "Particle")
            {
                return Instantiate(particlePrefab, position, rotation);
            } else if (tag == "TNT")
            {
                return tntFactory.CreateBlock("TNT", position);
            }
        }

        var objToSpawn = poolDictionary[tag].Dequeue();
        objToSpawn.transform.position = position;
        objToSpawn.transform.rotation = rotation;
        objToSpawn.transform.SetParent(objToSpawn.GetComponent<RectTransform>() != null ? null : blocks);
        objToSpawn.SetActive(true);

        return objToSpawn;
    }

    /// <summary>
    /// Returns an object to the appropriate pool.
    /// </summary>
    /// <param name="tag">The pool's tag to which the object should be returned.</param>
    /// <param name="objectToReturn">The object to return to the pool.</param>
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

    /// <summary>
    /// Represents a single pool in the object pooler.
    /// </summary>
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }
}
