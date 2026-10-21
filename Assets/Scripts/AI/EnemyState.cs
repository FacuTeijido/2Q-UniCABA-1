/*

 Author: Facu Teijido
 Email: facundoteijido2003@gmail.com

 Creation Date: 2026-10-15 

 

*/public abstract class EnemyState
{
    protected readonly Enemy enemy;

    protected EnemyState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public virtual void Enter() { }      // al entrar al estado
    public abstract void Tick();         // cada frame
    public virtual void Exit() { }       // al salir
}