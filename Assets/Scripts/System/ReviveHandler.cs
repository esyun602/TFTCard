public class ReviveHandler
{
    private bool canGlobalRevive;
    public void SetGlobalRevive()
    {
        canGlobalRevive = true;
    }
    
    public bool TryRevive(IBattleObject obj)
    {
        if (canGlobalRevive)
        {
            canGlobalRevive = false;
            obj.UnitCardBattleStat.Revive();
            return true;
        }

        return false;
    }

    public void Dispose()
    {
        
    }
}