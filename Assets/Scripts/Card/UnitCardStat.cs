
using System.Collections.Generic;

//todo: 우선은 정적 스탯 값은 아래 4가지만 가진다 가정
public class UnitCardStat : IStat
{
	public int MaxTurnCount { get; set; }
	public int MaxHp { get; set; }
	public int Attack { get; set; }
	public List<SynergyCategory> synergyList = new();
	private UnitStatSpec staticStatSpec;

	public UnitCardStat(UnitStatSpec statSpec)
	{
		staticStatSpec = statSpec;
		MaxTurnCount = statSpec.GetValueByValueType(BattleValueType.MaxTurnCount);
		MaxHp = statSpec.GetValueByValueType(BattleValueType.MaxHp);
		Attack = statSpec.GetValueByValueType(BattleValueType.Attack);
		synergyList = new(statSpec.SynergyList);
	}

	public int[] GetValuesByValueType(BattleValueType type)
	{
		switch (type)
		{
			case BattleValueType.MaxHp:
			case BattleValueType.Hp:
				return new int[] { MaxHp };
			case BattleValueType.TurnCount:
			case BattleValueType.MaxTurnCount:
				return new int[] { MaxTurnCount };
			case BattleValueType.Attack:
				return new int[] { Attack };
			default:
				return new int[] { };
		}
	}

	public void SetValuesByValueType(BattleValueType type, int[] newValues)
	{
		switch (type)
		{
			case BattleValueType.MaxHp:
			case BattleValueType.Hp:
				MaxHp = newValues[0];
				break;
			case BattleValueType.TurnCount:
			case BattleValueType.MaxTurnCount:
				MaxTurnCount = newValues[0];
				break;
			case BattleValueType.Attack:
				Attack = newValues[0];
				break;
		}
	}
}