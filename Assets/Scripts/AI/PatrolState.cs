/*

 Author: Facu Teijido
 Email: facundoteijido2003@gmail.com

 Creation Date: 2026-10-15 

 

*/using UnityEngine;
using UnityEngine.AI;

public class PatrolState : EnemyState
{
    const float ArriveDistance = 1.5f;
    const float MaxPatrolTime = 8f; // por si se traba y no llega nunca

    Vector3 destination;
    float timer;

    public PatrolState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        Vector2 offset = Random.insideUnitCircle * enemy.Data.patrolRadius;
        destination = enemy.HomePosition + new Vector3(offset.x, 0f, offset.y);
        timer = MaxPatrolTime;
    }

    public override void Tick()
    {
        if (enemy.CanSeeTarget)
        {
            enemy.ChangeState(enemy.StateChase);
            return;
        }

        float speed = enemy.Data.moveSpeed * enemy.Data.patrolSpeedMultiplier;
        enemy.MoveTowards(destination, speed);

        Vector3 flat = destination - enemy.transform.position;
        flat.y = 0f;
        timer -= Time.deltaTime;

        if (flat.magnitude <= ArriveDistance || timer <= 0f)
        
            enemy.ChangeState(enemy.StateIdle);
    }
    // Prueba hasta 10 puntos al azar y se queda con el primero que esté sobre el NavMesh
    Vector3 PickDestination()
    {
        Vector3 point = enemy.HomePosition;

        for (int i = 0; i < 10; i++)
        {
            Vector2 offset = Random.insideUnitCircle * enemy.Data.patrolRadius;
            point = enemy.HomePosition + new Vector3(offset.x, 0f, offset.y);

            if (NavMesh.SamplePosition(point, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                return hit.position;
        }

        return point;
    }
}