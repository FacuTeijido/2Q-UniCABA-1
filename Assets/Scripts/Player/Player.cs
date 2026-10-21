/*

 Author: Facu Teijido
 Email: facundoteijido2003@gmail.com

 Creation Date: 2026-10-13 

 

*/using UnityEngine;

public class Player : Character
{
    TankMovement movement;
    TankShooter shooter;
    TankTurret turret;
    Rigidbody rb;

    protected override void Awake()
    {
        base.Awake(); // importante: inicializa la vida

        movement = GetComponent<TankMovement>();
        shooter = GetComponent<TankShooter>();
        turret = GetComponentInChildren<TankTurret>();
        rb = GetComponent<Rigidbody>();
    }

    protected override void Die()
    {
        SetControlEnabled(false);
        // Más adelante: explosión, sonido, etc.
    }

    public void Respawn(Transform spawn)
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.position = spawn.position;
        rb.rotation = spawn.rotation;
        transform.SetPositionAndRotation(spawn.position, spawn.rotation);

        Revive();
        SetControlEnabled(true);
    }

    public void SetControlEnabled(bool value)
    {
        if (movement != null) movement.enabled = value;
        if (shooter != null) shooter.enabled = value;
        if (turret != null) turret.enabled = value;
    }

    // Para probar: clic derecho sobre el componente Player en Play > "Test: 50 de daño"
    [ContextMenu("Test: 50 de daño")]
    void DebugDamage() => TakeDamage(50f);
}