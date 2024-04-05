using UnityEngine;

/// <summary>
/// A generic singleton class that ensures only one instance of a MonoBehaviour-derived class exists in the scene.
/// </summary>
/// <typeparam name="T">The type of the singleton instance.</typeparam>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    /// <summary>
    /// The single instance of the class.
    /// </summary>
    private static T instance;

    /// <summary>
    /// Lock object for thread-safe initialization.
    /// </summary>
    private static readonly object syncLock = new object();

    /// <summary>
    /// Public accessor for the instance.
    /// </summary>
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

    /// <summary>
    /// Finds the instance in the scene or logs an error if none is found.
    /// </summary>
    /// <returns>The found instance of type T.</returns>
    private static T FindInstance()
    {
        T foundObject = FindObjectOfType<T>();
        if (foundObject == null)
        {
            Debug.LogError($"Instance of type '{typeof(T)}' not located in the scene.");
        }
        return foundObject;
    }

    /// <summary>
    /// Ensures that only one instance of this Singleton exists.
    /// </summary>
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
