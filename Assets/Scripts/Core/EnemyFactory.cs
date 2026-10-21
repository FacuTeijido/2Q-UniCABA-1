/*

 Author: Facu Teijido
 Email: facundoteijido2003@gmail.com

 Creation Date: 2026-10-15 

 

*/using System;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType { Melee, Ranged, Charger }

public class EnemyFactory : MonoBehaviour
{
    public static EnemyFactory Instance { get; private set; }

    [Serializable]
    public struct EnemyEntry
    {
        public EnemyType type;
        public Enemy prefab;
        public int prewarmCount; // cuántos crear por adelantado
    }

    [SerializeField] EnemyEntry[] entries;

    readonly Dictionary<EnemyType, Enemy> prefabs = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        foreach (EnemyEntry e in entries)
        {
            if (e.prefab != null) prefabs[e.type] = e.prefab;
        }
    }

    void Start()
    {
        // Prewarm: crea los enemigos antes de que haga falta (evita tirones al spawnear)
        foreach (EnemyEntry e in entries)
        {
            if (e.prefab != null && e.prewarmCount > 0)
                PoolManager.Instance.Prewarm(e.prefab.gameObject, e.prewarmCount);
        }
    }

    // El Factory Method: pide un tipo, devuelve un Enemy listo
    public Enemy Create(EnemyType type, Vector3 position, Quaternion rotation)
    {
        if (!prefabs.TryGetValue(type, out Enemy prefab))
        {
            Debug.LogError($"EnemyFactory: no hay prefab registrado para {type}.");
            return null;
        }

        GameObject obj = PoolManager.Instance.Get(prefab.gameObject, position, rotation);
        return obj.GetComponent<Enemy>();
    }
}