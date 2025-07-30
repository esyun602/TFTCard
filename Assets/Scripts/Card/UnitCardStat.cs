
using System.Collections.Generic;

public class UnitCardStat : IStat
{
	public int MaxTurnCount { get; set; }
	public int MaxHp { get; set; }
	public int Attack { get; set; }
	public int Cost { get; set; }
	public List<SynergyCategory> synergyList = new();
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

	public int[] GetValuesByValueType(BattleValueType type)
	{
		switch (type)
		{
			case BattleValueType.MaxHp:
				return new int[]{ MaxHp };
			case BattleValueType.Hp:
				return new int[] { MaxHp };
			case BattleValueType.TurnCount:
				return new int[] { MaxTurnCount };
			case BattleValueType.MaxTurnCount:
				return new int[] { MaxTurnCount };
			case BattleValueType.Attack:
				return new int[] { Attack };
			case BattleValueType.Cost:
				return new int[] { Cost };
			default:
				return new int[] { };
		}
	}
}