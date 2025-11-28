using System.Collections.Generic;
using Unity.VisualScripting;

public class NobleSynergy : AddOptionBattleSynergy
{
    private SynergySpec spec;
    private int[] goldGain = { 0,10,15,20 };

    public NobleSynergy(NobleSynergySpec spec) : base(spec)
    {
        this.spec = spec;
        memberList = new();
    }

    public int Level => spec.GetGrade(memberList.Count);

    public override void Activate()
    {
        Game.Instance.GetGameMode<BattleStageGameMode>().BattleGlobalModifier.RewardGoldAdd += goldGain[Level];
        base.Activate();
    }

    public override void Deactivate()
    {
        Game.Instance.GetGameMode<BattleStageGameMode>().BattleGlobalModifier.RewardGoldAdd -= goldGain[Level];
        base.Deactivate();
    }
}