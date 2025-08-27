using Unity.VisualScripting;
using UnityEngine;

public class UnitSkillCardInHand : BattleCardObjectInHand
{
	private const string cardPrefabPath = "Card/UnitActSkillCardPrefab";
	private UnitSkillCard unitSkillCard;
	protected override SkillCardBase TargetCard => unitSkillCard;
	public override ObjectType CardType => battleStat.Owner.ObjectType;
	public override IStat Stat => battleStat;
	private UnitSkillCardBattleStat battleStat;

	public void SetOwner(IBattleObject bo)
	{
		battleStat.Owner = bo;
	}

	private UnitSkillCardInHand()
	{
		
	}

	public static UnitSkillCardInHand Instantiate(UnitSkillCard targetSkillCard, UnitSkillCardBattleStat skillCardStat)
	{
		var cardObject = GameObject.Instantiate(Resources.Load(cardPrefabPath)).AddComponent<UnitSkillCardInHand>();
		cardObject.gameObject.SetActive(false);
		cardObject.unitSkillCard = targetSkillCard;
		cardObject.battleStat = skillCardStat;
		cardObject.unitSkillCard.Action.SetCardBattleStat(skillCardStat);

		return cardObject;
	}
}