/*

 Author: Facu Teijido
 Email: facundoteijido2003@gmail.com

 Creation Date: 2026-10-15 

 

*/public class StateMachine
{
    public EnemyState Current { get; private set; }

    public void ChangeState(EnemyState next)
    {
        if (next == null || next == Current) return;

        Current?.Exit();
        Current = next;
        Current.Enter();
    }

    public void Clear()
    {
        Current?.Exit();
        Current = null;
    }

    public void Tick()
    {
        Current?.Tick();
    }
}