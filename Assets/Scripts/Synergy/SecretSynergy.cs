using System.Collections.Generic;
using MessageSystem;

public class SecretSynergy : IGlobalSynergy
{
    public int Level => synergySpec.GetGrade(memberList.Count);
    private SynergySpec synergySpec;
    private List<UnitCard> memberList;
    private Dictionary<UnitCard, List<UnitSkillCard>> originUnitSkillCardCache;
    private Dictionary<UnitCard, List<UnitSkillCard>> upgradeUnitSkillCardCache;

    public SecretSynergy(SynergySpec synergySpec)
    {
        this.synergySpec = synergySpec;
    }

    private bool IsActivated(int lv) => lv is 1 or 3;
    public void Initialize()
    {
        originUnitSkillCardCache = new();
        upgradeUnitSkillCardCache = new();
        memberList = new();
    }

    public void AddMember(UnitCard target)
    {
        var upgradeApplied = IsActivated(Level);
        memberList.Add(target);
        originUnitSkillCardCache[target] = new List<UnitSkillCard>(target.UnitSkillCard);
        if (!upgradeApplied && IsActivated(Level))
        {
            UpgradeMemberCards();
        }
        else if(upgradeApplied && !IsActivated(Level))
        {
            DowngradeMemberCards();
        }
    }

    private void UpgradeMemberCards()
    {
        foreach (var member in memberList)
        {
            member.UnitSkillCard.Clear();
            
            if (!upgradeUnitSkillCardCache.ContainsKey(member))
            {
                foreach (var str in member.UnitCardStaticSpec.ExtraParams.GetStringArray("UpgradeCardNames")) 
                {
                    member.UnitSkillCard.Add(new UnitSkillCard(GameDataSystem.Instance.GetGameData<CardData>().GetUnitSkillCardSpecByName(str), member));
                }
                upgradeUnitSkillCardCache[member] = new List<UnitSkillCard>(member.UnitSkillCard);
            }
            else
            {
                foreach (var card in  upgradeUnitSkillCardCache[member])
                {
                    member.UnitSkillCard.Add(card);
                }
            }
        }
        
        NoticeSystem.Instance.Publish(new UnitSkillCardUpdateNotice());
    }

    private void DowngradeMemberCards()
    {
        foreach (var member in memberList)
        {
            member.UnitSkillCard.Clear();
            
            foreach (var card in  originUnitSkillCardCache[member])
            {
                member.UnitSkillCard.Add(card);
            }
        }
        
        NoticeSystem.Instance.Publish(new UnitSkillCardUpdateNotice());
    }

    public void RemoveMember(UnitCard target)
    {
        var upgradeApplied = IsActivated(Level);
        
        if (upgradeApplied && !IsActivated(synergySpec.GetGrade(memberList.Count - 1)))
        {
            DowngradeMemberCards();
            memberList.Remove(target);
            originUnitSkillCardCache.Remove(target);
        }
        else if (!upgradeApplied && IsActivated(synergySpec.GetGrade(memberList.Count - 1)))
        {
            memberList.Remove(target);
            UpgradeMemberCards();
            originUnitSkillCardCache.Remove(target);
        }
        else
        {
            memberList.Remove(target);
            originUnitSkillCardCache.Remove(target);
        }
    }

    public void Dispose()
    {
        originUnitSkillCardCache = null;
        upgradeUnitSkillCardCache = null;
        memberList = null;
    }
}