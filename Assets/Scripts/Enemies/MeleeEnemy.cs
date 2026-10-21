/*

 Author: Facu Teijido
 Email: facundoteijido2003@gmail.com

 Creation Date: 2026-10-13 

 

*/public class MeleeEnemy : Enemy
{
    protected override void Attack()
    {
        // Golpea a cualquier IDamageable, sin saber qué es
        target.TakeDamage(data.attackDamage);
    }
}