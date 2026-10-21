/*

 Author: Facu Teijido
 Email: facundoteijido2003@gmail.com

 Creation Date: 2026-10-13 

 

*/using System;
using UnityEngine;

public abstract class Character : MonoBehaviour, IDamageable
{
    [SerializeField] protected float maxHealth = 100f;
    [field: SerializeField] public float CurrentHealth { get; private set; }
    //public float CurrentHealth { get; private set; }
    public float MaxHealth => maxHealth;
    public bool IsDead { get; private set; }

    // Eventos: avisan sin conocer a quién les escucha
    public event Action<float, float> OnHealthChanged; // (vida actual, vida máxima)
    public event Action<Character> OnDied;

    protected virtual void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (IsDead || amount <= 0f) return;

        CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

        if (CurrentHealth <= 0f)
        {
            IsDead = true;
            OnDied?.Invoke(this);
            Die();
        }
    }

    public void Revive()
    {
        IsDead = false;
        CurrentHealth = maxHealth;
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }

    // Cada clase hija define qué significa "morir"
    protected abstract void Die();
}