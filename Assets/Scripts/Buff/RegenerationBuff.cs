using MessageSystem;

public class RegenerationBuff : BuffBase
{
    public RegenerationBuff(int lv)
    {
        Level = lv;
    }

    public override BuffType DefaultType => BuffType.Positive | BuffType.BlockOptionAdd;
    public override UnitValueType ControlUnitValueType => UnitValueType.Regeneration;

    protected override void OnAdd()
    {
        NoticeSystem.Instance.Subscribe<PlayerTurnEndNotice>(OnTurnEnd);
    }

    private void OnTurnEnd(PlayerTurnEndNotice m)
    {
        Game.Instance.GetGameMode<BattleStageGameMode>().TurnSystem.RegisterAutoTurnRoutine(IUpdatableRoutineExtensions.GenerateRunAfterTime(1f, Regenerate));
    }

    private void Regenerate()
    {
        if (target == null) return;
        target.Heal(new HealInfo()
        {
            HealAmount = Level--,
        });

        if (Level == 0)
        {
            target.UnitCardBattleStat.RemoveBuff<RegenerationBuff>();
        }
    }

    protected override void OnRemove()
    {
        NoticeSystem.Instance.Unsubscribe<PlayerTurnEndNotice>(OnTurnEnd);
    }

    protected override bool TryStackImpl(IBuff buff)
    {
        var canStack = buff is RegenerationBuff;
        if (canStack)
        {
            Level += buff.Level;
        }

        return canStack;
    }

    public override string Keyword => "Regeneration";
}