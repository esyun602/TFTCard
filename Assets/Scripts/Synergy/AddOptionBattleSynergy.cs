using System;
using System.Collections.Generic;

public class AddOptionBattleSynergy : IBattleSynergy
{
    protected HashSet<IBattleObject> memberList;
    private AddOptionBattleSynergySpec spec;

    public AddOptionBattleSynergy(AddOptionBattleSynergySpec spec)
    {
        this.spec = spec;
        memberList = new();
    }

    public int Level => spec.GetGrade(memberList.Count);

    public virtual void Activate()
    {
        if (Level >= 1)
        {
            foreach (var member in memberList)
            {
                AddOptionToObject(member);
            }
        }
    }

    public virtual void Deactivate()
    {
        foreach (var m in memberList)
        {
            RemoveOptionFromObject(m);
        }
        memberList = null;
    }

    private void AddOptionToObject(IBattleObject obj)
    {
        var option = spec.CreateOption();
        option.Level = Level;
        obj.UnitCardBattleStat.AddOption(option);
    }

    private void RemoveOptionFromObject(IBattleObject obj)
    {
        obj.UnitCardBattleStat.RemoveOption<AlchemistOption>();
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