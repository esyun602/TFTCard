using Unity.VisualScripting;
using UnityEngine;

public class TacticsCardInHand : BattleCardObjectInHand
{
	private const string cardPrefabPath = "Card/SkillCardPrefab";
	public override SkillCardBase TargetCard => targetCard;
	private TacticsCard targetCard;
	public override ObjectType CardType => ObjectType.Ally;
	public override IStat Stat => battleStat;
	private TacticsCardBattleStat battleStat;

	private TacticsCardInHand()
	{
		
	}

	public static TacticsCardInHand Instantiate(TacticsCard targetSkillCard, TacticsCardBattleStat skillCardStat)
	{
		var cardObject = GameObject.Instantiate(Resources.Load(cardPrefabPath)).AddComponent<TacticsCardInHand>();
		cardObject.gameObject.SetActive(false);
		cardObject.targetCard = targetSkillCard;
		cardObject.battleStat = skillCardStat;
		cardObject.targetCard.Action.SetCardBattleStat(skillCardStat);

		return cardObject;
	}
}