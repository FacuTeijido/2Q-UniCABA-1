/*

 Author: Facu Teijido
 Email: facundoteijido2003@gmail.com

 Creation Date: 2026-10-13 

 

*/using UnityEngine;
using UnityEngine.InputSystem;

public class TankShooter : MonoBehaviour
{
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform firePoint;
    [SerializeField] float fireRate = 0.4f; // segundos entre disparos
    [SerializeField] int prewarmCount = 10;
    [SerializeField] float shotNoiseRadius = 30f;
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] AudioClip shootSound;
    float nextFireTime;

    void Start()
    {
        PoolManager.Instance.Prewarm(projectilePrefab, prewarmCount);
    }

    void Update()
    {
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.isPressed && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        GameObject bullet = PoolManager.Instance.Get(projectilePrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<Projectile>().Init(transform.root);
        NoiseSystem.Emit(firePoint.position, shotNoiseRadius);
        if (muzzleFlash != null)
        {
            muzzleFlash.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            muzzleFlash.Play();
        }
        if (shootSound != null)
        {
            AudioSource.PlayClipAtPoint(shootSound, firePoint.position);
        }
    }
}