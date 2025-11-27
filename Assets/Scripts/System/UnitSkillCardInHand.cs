using Unity.VisualScripting;
using UnityEngine;

public class UnitSkillCardInHand : BattleCardObjectInHand
{
	private const string allyCardPrefabPath = "Card/UnitActSkillCardPrefab";
	private const string enemyCardPrefabPath = "Card/EnemySkillCard";
	private UnitSkillCard unitSkillCard;
	public override SkillCardBase TargetCard => unitSkillCard;
	public override ObjectType CardType => battleStat.Owner.ObjectType;
	public override SkillCardBattleStat Stat => battleStat;
	private UnitSkillCardBattleStat battleStat;

	/*public void SetOwner(IBattleObject bo)
	{
		battleStat.Owner = bo;
	}*/

	//todo: fix, 더 윗단에 적용해도 될듯
	private bool dead = false;
	public void SetDeadState()
	{
		dead = true;
	}

	public void RemoveDeadState()
	{
		dead = false;
	}

	protected override bool CanSelect()
	{
		//todo: dead로?	
		return base.CanSelect() && !dead;
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