//todo: type에 따른 dictionary 방식으로 수정하는게 나을듯
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

	public SkillCardStat(SkillCardStatSpec statSpec)
	{
		TurnCountValue = statSpec.turnCountValue;
		HpValue = statSpec.hpValue;
		AttackValue = statSpec.attackValue;
		CostValue = statSpec.costValue;
		ShieldValue = statSpec.shieldValue;
	}

	public int[] GetValuesByValueType(ValueType type)
	{
		switch (type)
		{
			case ValueType.MaxHp:
			case ValueType.Hp:
				return new int[] { HpValue };
			case ValueType.TurnCount:
			case ValueType.MaxTurnCount:
				return new int[] { TurnCountValue };
			case ValueType.Attack:
				return new int[] { AttackValue };
			case ValueType.Cost:
				return new int[] { CostValue };
			case ValueType.Burn:
				return new int[] { BurnValue };
			case ValueType.Catalyst:
				return new int[] { CatalystValue };
			case ValueType.Stun:
				return new int[] { StunValue };
			case ValueType.Dodge:
				return new int[] { DodgeValue };
			default:
				return new int[] { };
		}
	}
}