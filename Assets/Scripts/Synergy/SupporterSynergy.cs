using System.Collections.Generic;
using MessageSystem;

public class SupporterSynergy : IBattleSynergy
{
    private HashSet<IBattleObject> memberList;
    private SynergySpec spec;

    public SupporterSynergy(SynergySpec spec)
    {
        this.spec = spec;
        memberList = new();
    }

    public int Level => spec.GetGrade(memberList.Count);

    public void Activate()
    {
        if (Level >= 1)
        {
            NoticeSystem.Instance.Subscribe<HealNotice>(OnHeal);
        }
    }

    private void OnHeal(HealNotice m)
    {
        if (m.Target.ObjectType == ObjectType.Ally)
        {
            m.Target.UnitCardBattleStat.Purify();
        }
    }

    public void Deactivate()
    {
        if (Level >= 1)
        {
            NoticeSystem.Instance.Unsubscribe<HealNotice>(OnHeal);
        }
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