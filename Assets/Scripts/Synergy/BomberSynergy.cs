using System.Collections.Generic;

public class BomberSynergy : IBattleSynergy
{
    private HashSet<IBattleObject> memberList;
    private SynergySpec spec;

    public BomberSynergy(SynergySpec spec)
    {
        this.spec = spec;
        memberList = new();
    }

    public int Level => spec.GetGrade(memberList.Count);

    public void Activate()
    {
        if (Level >= 1)
        {
            foreach (var member in memberList)
            {
                member.UnitCardBattleStat.AddValueByValueType(SkillValueType.BombDamage, 1);
            }
        }
    }

    public void Deactivate()
    {
        if (Level >= 1)
        {
            foreach (var member in memberList)
            {
                member.UnitCardBattleStat.AddValueByValueType(SkillValueType.BombDamage, -1);
            }
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