/*

 Author: Facu Teijido
 Email: facundoteijido2003@gmail.com

 Creation Date: 2026-10-14 

 

*/using System.Collections;
using UnityEngine;

public class ChargerEnemy : Enemy
{
    [Header("Embestida")]
    [SerializeField] float windUpTime = 0.7f;      // tiempo de preparación
    [SerializeField] float chargeSpeed = 14f;
    [SerializeField] float chargeDuration = 0.8f;
    [SerializeField] float recoverTime = 1.2f;     // queda vulnerable

    bool isCharging;
    bool hasHitThisCharge;

    protected override void Attack()
    {
        StartCoroutine(ChargeRoutine());
    }

    IEnumerator ChargeRoutine()
    {
        isBusy = true;
        hasHitThisCharge = false;

        // 1. Preparación: se frena y apunta al jugador
        Stop();
        float t = 0f;
        while (t < windUpTime)
        {
            FaceTowards(target.transform.position);
            t += Time.deltaTime;
            yield return null;
        }

        // 2. Embestida: la dirección se fija ahora, así se puede esquivar
        Vector3 dir = target.transform.position - transform.position;
        dir.y = 0f;
        isCharging = true;

        t = 0f;
        while (t < chargeDuration && !hasHitThisCharge)
        {
            Move(dir, chargeSpeed);
            t += Time.deltaTime;
            yield return null;
        }
        isCharging = false;

        // 3. Recuperación
        Stop();
        yield return new WaitForSeconds(recoverTime);

        isBusy = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isCharging || hasHitThisCharge) return;

        Player player = collision.collider.GetComponentInParent<Player>();
        if (player == null) return;

        hasHitThisCharge = true;
        player.TakeDamage(data.attackDamage);
    }
    protected override void OnSpawned()
    {
        isCharging = false;
        hasHitThisCharge = false;
    }
}