
using System.Collections.Generic;

public class UnitCardStat : IStat
{
	public int MaxTurnCount { get; set; }
	public int MaxHp { get; set; }
	public int Attack { get; set; }
	public int Cost { get; set; }
	public List<Synergy> synergyList = new();
	private UnitCardStatSpec staticStatSpec;

	public UnitCardStat(UnitCardStatSpec statSpec)
	{
		staticStatSpec = statSpec;
		MaxTurnCount = statSpec.turnCount;
		MaxHp = statSpec.hp;
		Attack = statSpec.attack;
		Cost = statSpec.cost;
		synergyList = new(statSpec.synergy);
	}

	public int[] GetValuesByValueType(ValueType type)
	{
		switch (type)
		{
			case ValueType.MaxHp:
				return new int[]{ MaxHp };
			case ValueType.Hp:
				return new int[] { MaxHp };
			case ValueType.TurnCount:
				return new int[] { MaxTurnCount };
			case ValueType.MaxTurnCount:
				return new int[] { MaxTurnCount };
			case ValueType.Attack:
				return new int[] { Attack };
			case ValueType.Cost:
				return new int[] { Cost };
			default:
				return new int[] { };
		}
	}
}