using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    readonly Dictionary<GameObject, Queue<GameObject>> pools = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Crea objetos por adelantado para no instanciar en pleno juego
    public void Prewarm(GameObject prefab, int count)
    {
        for (int i = 0; i < count; i++)
            Release(CreateNew(prefab));
    }

    public GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        Queue<GameObject> queue = GetQueue(prefab);
        GameObject obj = queue.Count > 0 ? queue.Dequeue() : CreateNew(prefab);

        obj.transform.SetParent(null);
        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true); // OnEnable corre acá, con la posición ya puesta
        return obj;
    }

    public void Release(GameObject obj)
    {
        PooledObject pooled = obj.GetComponent<PooledObject>();
        if (pooled == null)
        {
            Destroy(obj); // no vino del pool
            return;
        }

        obj.SetActive(false);
        obj.transform.SetParent(transform);
        GetQueue(pooled.Prefab).Enqueue(obj);
    }

    GameObject CreateNew(GameObject prefab)
    {
        GameObject obj = Instantiate(prefab, transform);
        obj.AddComponent<PooledObject>().Prefab = prefab;
        obj.SetActive(false);
        return obj;
    }

    Queue<GameObject> GetQueue(GameObject prefab)
    {
        if (!pools.TryGetValue(prefab, out Queue<GameObject> queue))
        {
            queue = new Queue<GameObject>();
            pools[prefab] = queue;
        }
        return queue;
    }
}