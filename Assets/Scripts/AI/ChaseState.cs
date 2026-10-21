/*

 Author: Facu Teijido
 Email: facundoteijido2003@gmail.com

 Creation Date: 2026-10-15 

 

*/using UnityEngine;

public class ChaseState : EnemyState
{
    float timeSinceSeen;

    public ChaseState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        timeSinceSeen = 0f;
    }

    public override void Tick()
    {
        if (enemy.CanSeeTarget)
        {
            timeSinceSeen = 0f;

            if (enemy.DistanceToTarget() <= enemy.Data.attackRange)
            {
                enemy.ChangeState(enemy.StateAttack);
                return;
            }

            enemy.MoveTowards(enemy.Target.transform.position);
            return;
        }

        // Lo perdió de vista: va al último lugar donde lo vio y busca un rato
        timeSinceSeen += Time.deltaTime;
        if (timeSinceSeen > enemy.Data.memoryTime)
        {
            enemy.ChangeState(enemy.StatePatrol);
            return;
        }

        Vector3 flat = enemy.LastKnownPosition - enemy.transform.position;
        flat.y = 0f;

        if (flat.magnitude > 2f) enemy.MoveTowards(enemy.LastKnownPosition);
        else enemy.Stop();
    }
    public void RefreshSearch()
    {
        timeSinceSeen = 0f;
    }
}