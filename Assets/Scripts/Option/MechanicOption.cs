using MessageSystem;

public class MechanicOption : IOption
{
    private IBattleObject target;
    public int Level { get; set; }
    
    public void OnAdd(IBattleObject target)
    {
        this.target = target;
        NoticeSystem.Instance.Subscribe<SkillHandCardStartUseNotice>(OnCardUse);
    }

    public void OnRemove()
    {
        NoticeSystem.Instance.Unsubscribe<SkillHandCardStartUseNotice>(OnCardUse);
    }

    private void OnCardUse(SkillHandCardStartUseNotice m)
    {
        if (m.SelectedCard.Stat is not UnitSkillCardBattleStat us || us.Owner != target) return;
        
        var map = Game.Instance.GetGameMode<BattleStageGameMode>().BattleStage.Map;
        map.GetFirstObjectInRow(map.GetTileOfBattleObject(target)).UnitCardBattleStat.AddValueByValueType(UnitValueType.Shield, 1);
        
    }
}