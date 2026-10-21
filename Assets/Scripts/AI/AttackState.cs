/*

 Author: Facu Teijido
 Email: facundoteijido2003@gmail.com

 Creation Date: 2026-10-15 

 

*/using UnityEngine;

public class AttackState : EnemyState
{
    // Sale del ataque un poco más lejos de donde entró: evita que parpadee entre estados
    const float ExitRangeMultiplier = 1.15f;

    public AttackState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.Stop();
    }

    public override void Tick()
    {
        if (enemy.IsBusy) return; // en medio de una embestida, no interrumpir

        bool inRange = enemy.DistanceToTarget() <= enemy.Data.attackRange * ExitRangeMultiplier;

        if (!enemy.CanSeeTarget || !inRange)
        {
            enemy.ChangeState(enemy.StateChase);
            return;
        }

        enemy.Stop();
        enemy.FaceTowards(enemy.Target.transform.position);
        enemy.TryAttack(); // acá se dispara el Attack() polimórfico de cada tipo
    }
}