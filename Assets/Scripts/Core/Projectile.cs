/*

 Author: Facu Teijido
 Email: facundoteijido2003@gmail.com

 Creation Date: 2026-10-13 

 

*/
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField] float speed = 20f;
    [SerializeField] float damage = 10f;
    [SerializeField] float lifeTime = 3f;

    Rigidbody rb;
    Transform owner;
    float currentDamage;
    bool fromEnemy;
    float timer;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Los parámetros opcionales mantienen compatible a TankShooter: Init(transform.root)
    public void Init(Transform ownerRoot, bool isEnemyBullet = false, float damageOverride = -1f)
    {
        owner = ownerRoot;
        fromEnemy = isEnemyBullet;
        currentDamage = damageOverride >= 0f ? damageOverride : damage;
    }

    void OnEnable()
    {
        // Valores por defecto: Init los pisa justo después de salir del pool
        timer = lifeTime;
        currentDamage = damage;
        fromEnemy = false;
        rb.linearVelocity = transform.forward * speed;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f) Despawn();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;
        if (owner != null && other.transform.root == owner) return;

        IDamageable target = other.GetComponentInParent<IDamageable>();
        if (fromEnemy && target is Enemy) return;

        if (target != null) target.TakeDamage(currentDamage);
        Despawn();
    }

    void Despawn()
    {
        PoolManager.Instance.Release(gameObject);
    }
}