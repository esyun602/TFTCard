using Unity.VisualScripting;
using UnityEngine;

public class UnitSkillCardInHand : BattleCardObjectInHand
{
	private const string allyCardPrefabPath = "Card/UnitActSkillCardPrefab";
	private const string enemyCardPrefabPath = "Card/EnemySkillCard";
	private UnitSkillCard unitSkillCard;
	protected override SkillCardBase TargetCard => unitSkillCard;
	public override ObjectType CardType => battleStat.Owner.ObjectType;
	public override IStat Stat => battleStat;
	private UnitSkillCardBattleStat battleStat;

	public void SetOwner(IBattleObject bo)
	{
		battleStat.Owner = bo;
	}

	protected override bool CanSelect()
	{
		//todo: dead로?
		return base.CanSelect() && (battleStat.Owner != null);
	}

	private UnitSkillCardInHand()
	{
		
	}

	public static UnitSkillCardInHand InstantiateForAlly(UnitSkillCard targetSkillCard, UnitSkillCardBattleStat skillCardStat)
	{
		var cardObject = GameObject.Instantiate(Resources.Load(allyCardPrefabPath)).AddComponent<UnitSkillCardInHand>();
		cardObject.gameObject.SetActive(false);
		cardObject.unitSkillCard = targetSkillCard;
		cardObject.battleStat = skillCardStat;
		cardObject.unitSkillCard.Action.SetCardBattleStat(skillCardStat);

		return cardObject;
	}
	
	public static UnitSkillCardInHand InstantiateForEnemy(UnitSkillCard targetSkillCard, UnitSkillCardBattleStat skillCardStat)
	{
		var cardObject = GameObject.Instantiate(Resources.Load(enemyCardPrefabPath)).AddComponent<UnitSkillCardInHand>();
		cardObject.gameObject.SetActive(false);
		cardObject.unitSkillCard = targetSkillCard;
		cardObject.battleStat = skillCardStat;
		cardObject.unitSkillCard.Action.SetCardBattleStat(skillCardStat);

		return cardObject;
	}
}