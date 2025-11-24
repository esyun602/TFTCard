using System;
using System.Collections.Generic;

public class ClockSynergy : IBattleSynergy
{
    private HashSet<IBattleObject> memberList;
    private SynergySpec spec;
    private bool IsActivated => spec.GetGrade(memberList.Count) >= 1;

    public ClockSynergy(SynergySpec spec)
    {
        this.spec = spec;
        memberList = new();
    }

    public int Level => spec.GetGrade(memberList.Count);

    public void Activate()
    {
        if (IsActivated)
        {
            Game.Instance.GetGameMode<BattleStageGameMode>().BattleFieldSystem.ReviveHandler.SetGlobalRevive();
        }
    }

    public void Deactivate()
    {
    }

    private void AddOptionToObject(IBattleObject obj)
    {
    }

    private void RemoveOptionFromObject(IBattleObject obj)
    {
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