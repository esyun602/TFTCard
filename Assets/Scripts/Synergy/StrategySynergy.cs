using System.Collections.Generic;
using MessageSystem;
using Unity.VisualScripting;

public class StrategySynergy : IBattleSynergy
{
    private bool cardCostActivated;
    private HashSet<IBattleObject> memberList;
    private SynergySpec spec;

    public StrategySynergy(SynergySpec spec)
    {
        this.spec = spec;
        memberList = new();
    }

    public int Level => spec.GetGrade(memberList.Count);

    public void Activate()
    {
        if (Level >= 1)
        {
     	    NoticeSystem.Instance.Subscribe<PlayerTurnStartNotice>(OnPlayerTurnStart);
            NoticeSystem.Instance.Subscribe<SkillHandCardStartUseNotice>(OnCardUse);
        }   
    }

    private void OnPlayerTurnStart(PlayerTurnStartNotice m)
    {
        if (!cardCostActivated)
        {
            cardCostActivated = true;
            Game.Instance.GetGameMode<BattleStageGameMode>().BattleGlobalModifier.CardCostAdd -= 1;
        }
    }

    private void OnCardUse(SkillHandCardStartUseNotice m)
    {
        if (cardCostActivated)
        {
            cardCostActivated = false;
            Game.Instance.GetGameMode<BattleStageGameMode>().BattleGlobalModifier.CardCostAdd += 1;
        }
    }

    public void Deactivate()
    {
	if(Level < 1) return;
        if (cardCostActivated)
        {
            cardCostActivated = true;
            Game.Instance.GetGameMode<BattleStageGameMode>().BattleGlobalModifier.CardCostAdd -= 1;
        }
        
        NoticeSystem.Instance.Unsubscribe<PlayerTurnStartNotice>(OnPlayerTurnStart);
        NoticeSystem.Instance.Unsubscribe<SkillHandCardStartUseNotice>(OnCardUse);
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