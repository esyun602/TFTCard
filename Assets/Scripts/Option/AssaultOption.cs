using MessageSystem;
using System;

public class AssaultOption : IOption
{
    private IBattleObject target;
    public int Level { get; set; }
    private bool isAdded;
    
    public void OnAdd(IBattleObject target)
    {
        this.target = target;
        NoticeSystem.Instance.Subscribe<BattleObjectPosUpdatedNotice>(OnPosUpdate);
        
        var map = Game.Instance.GetGameMode<BattleStageGameMode>().BattleStage.Map;
        var tile = map.GetTileOfBattleObject(target);
        if (map.GetOrderInRow(tile) == 1)
        {
            isAdded = true;
            AddBuff();
        }
    }

    public void OnRemove()
    {
        NoticeSystem.Instance.Unsubscribe<BattleObjectPosUpdatedNotice>(OnPosUpdate);
        if (isAdded)
        {
            RemoveBuff();
        }
    }

    private void OnPosUpdate(BattleObjectPosUpdatedNotice m)
    {
        if (m.Target != target) return;
        var map = Game.Instance.GetGameMode<BattleStageGameMode>().BattleStage.Map;
        if (isAdded && map.GetOrderInRow(m.TargetTile) != 1)
        {
            isAdded = false;
            RemoveBuff();
        }
        else if (!isAdded && map.GetOrderInRow(m.TargetTile) == 1)
        {
            isAdded = true;
            AddBuff();
        }
    }

    private void AddBuff()
    {
        target.UnitCardBattleStat.AddBuff(new ValueAddAttackBuff(2), this);
    }

    private void RemoveBuff()
    {
        target.UnitCardBattleStat.RemoveBuff<ValueAddAttackBuff>(this);
    }
}