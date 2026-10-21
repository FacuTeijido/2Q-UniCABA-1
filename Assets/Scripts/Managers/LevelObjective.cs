/*

 Author: Facu Teijido
 Email: facundoteijido2003@gmail.com

 Creation Date: 2026-10-15 

 

*/using System;
using UnityEngine;

public class LevelObjective : MonoBehaviour
{
    // [field: SerializeField] permite ver los contadores en el Inspector mientras probás
    [field: SerializeField] public int AliveCount { get; private set; }
    [field: SerializeField] public int TotalCount { get; private set; }
    public bool IsCompleted { get; private set; }

    public event Action<int, int> OnProgressChanged; // (vivos, total): lo va a usar la UI
    public event Action OnCompleted;

    void OnEnable()
    {
        Enemy.OnEnemySpawned += HandleSpawned;
        Enemy.OnEnemyKilled += HandleKilled;
    }

    void OnDisable()
    {
        // Siempre desuscribirse de eventos estáticos: viven más que este objeto
        Enemy.OnEnemySpawned -= HandleSpawned;
        Enemy.OnEnemyKilled -= HandleKilled;
    }

    void HandleSpawned(Enemy enemy)
    {
        AliveCount++;
        TotalCount++;
        OnProgressChanged?.Invoke(AliveCount, TotalCount);
    }

    void HandleKilled(Enemy enemy)
    {
        AliveCount = Mathf.Max(0, AliveCount - 1);
        OnProgressChanged?.Invoke(AliveCount, TotalCount);

        if (AliveCount == 0 && !IsCompleted)
        {
            IsCompleted = true;
            OnCompleted?.Invoke();
        }
    }
}