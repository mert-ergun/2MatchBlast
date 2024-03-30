using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    private static readonly object syncLock = new object();

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                lock (syncLock)
                {
                    instance = FindInstance();
                }
            }
            return instance;
        }
    }

    private static T FindInstance()
    {
        T foundObject = FindObjectOfType<T>();
        if (foundObject == null)
        {
            Debug.LogError($"Instance of type '{typeof(T)}' not located in the scene.");
        }
        return foundObject;
    }

    protected virtual void Awake()
    {
        lock (syncLock)
        {
            if (instance != null && instance != this)
            {
                Debug.LogError($"Multiple instances of type '{typeof(T)}' detected. Retaining a single instance and discarding others.");
                Destroy(gameObject);
            }
            else
            {
                instance = this as T;
            }
        }
    }
}
