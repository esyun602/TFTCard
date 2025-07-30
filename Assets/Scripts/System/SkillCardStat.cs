//todo: type에 따른 dictionary 방식으로 수정하는게 나을듯
//todo: skill카드에서 value set 관련 수정
public class SkillCardStat : IStat
{
	public int TurnCountValue { get; set; }
	public int HpValue { get; set; }
	public int AttackValue { get; set; }
	public int CostValue { get; set; }
	public int ShieldValue { get; set; }	
	public int BurnValue { get; set; }
	public int CatalystValue { get; set; }
	public int StunValue { get; set; }
	public int DodgeValue { get; set; }
	//todo:fix
	public bool IsExhaustion { get; set; }

	public SkillCardStat(SkillCardStatSpec statSpec)
	{
		TurnCountValue = statSpec.turnCountValue;
		HpValue = statSpec.hpValue;
		AttackValue = statSpec.attackValue;
		CostValue = statSpec.costValue;
		ShieldValue = statSpec.shieldValue;
		
		//todo: fix
		IsExhaustion = statSpec.isExhaustion;
	}

	public int[] GetValuesByValueType(BattleValueType type)
	{
		switch (type)
		{
			case BattleValueType.MaxHp:
			case BattleValueType.Hp:
				return new int[] { HpValue };
			case BattleValueType.TurnCount:
			case BattleValueType.MaxTurnCount:
				return new int[] { TurnCountValue };
			case BattleValueType.Attack:
				return new int[] { AttackValue };
			case BattleValueType.Cost:
				return new int[] { CostValue };
			case BattleValueType.Burn:
				return new int[] { BurnValue };
			case BattleValueType.Catalyst:
				return new int[] { CatalystValue };
			case BattleValueType.Stun:
				return new int[] { StunValue };
			case BattleValueType.Dodge:
				return new int[] { DodgeValue };
			default:
				return new int[] { };
		}
	}
}