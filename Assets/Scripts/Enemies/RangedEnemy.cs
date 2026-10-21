/*

 Author: Facu Teijido
 Email: facundoteijido2003@gmail.com

 Creation Date: 2026-10-14 

 

*/using UnityEngine;

public class RangedEnemy : Enemy
{
    [Header("Disparo")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform firePoint;

    protected override void Attack()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError($"{name}: falta asignar el prefab de la bala.");
            return;
        }

        Vector3 origin = firePoint != null ? firePoint.position : transform.position + Vector3.up;

        // Apunta directo al jugador, sin depender de si ya terminó de girar
        Vector3 dir = target.transform.position - origin;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.01f) return;

        GameObject bullet = PoolManager.Instance.Get(projectilePrefab, origin, Quaternion.LookRotation(dir));
        bullet.GetComponent<Projectile>().Init(transform.root, true, data.attackDamage);
    }
}