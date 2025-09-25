using System.Collections.Generic;

public class NobleSynergy : IBattleSynergy
{
    private HashSet<IBattleObject> memberList;
    private SynergySpec spec;
    private int[] goldGain = { 0,10,20,30 };

    public NobleSynergy(SynergySpec spec)
    {
        this.spec = spec;
        memberList = new();
    }

    public int Level => spec.GetGrade(memberList.Count);

    public void Activate()
    {
        Game.Instance.GetGameMode<BattleStageGameMode>().BattleGlobalModifier.RewardGoldAdd += goldGain[Level];
    }

    public void Deactivate()
    {
        Game.Instance.GetGameMode<BattleStageGameMode>().BattleGlobalModifier.RewardGoldAdd -= goldGain[Level];
    }

    public void AddMember(IBattleObject obj)
    {
        memberList.Add(obj);
    }

    public void RemoveMember(IBattleObject obj)
    {
        memberList.Remove(obj);
    }
}