/*

 Author: Facu Teijido
 Email: facundoteijido2003@gmail.com

 Creation Date: 2026-10-15 

 

*/using UnityEngine;

public class IdleState : EnemyState
{
    float timer;

    public IdleState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.Stop();
        timer = Random.Range(1f, 3f);
    }

    public override void Tick()
    {
        if (enemy.CanSeeTarget)
        {
            enemy.ChangeState(enemy.StateChase);
            return;
        }

        timer -= Time.deltaTime;
        if (timer <= 0f) enemy.ChangeState(enemy.StatePatrol);
    }
}