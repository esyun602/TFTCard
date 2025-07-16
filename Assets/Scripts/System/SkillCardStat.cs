//todo: type에 따른 dictionary 방식으로 수정하는게 나을듯
public class SkillCardStat : IStat
{
	public int TurnCountValue { get; set; }
	public int HpValue { get; set; }
	public int AttackValue { get; set; }
	public int CostValue { get; set; }
	public int ShieldValue { get; set; }

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
			default:
				return new int[] { };
		}
	}
}