using System.Collections.Generic;

//todo: 우선은 정적 스탯 값은 아래 2가지만 가진다 가정
public class UnitCardStat : IStat
{
	public int MaxHp { get; set; }
	public int Attack { get; set; }
	public List<SynergyCategory> synergyList = new();
	private UnitStatSpec staticStatSpec;

	public UnitCardStat(UnitStatSpec statSpec)
	{
		staticStatSpec = statSpec;
		MaxHp = statSpec.GetValueByValueType(UnitValueType.MaxHp);
		Attack = statSpec.GetValueByValueType(UnitValueType.Attack);
		synergyList = new(statSpec.SynergyList);
	}

	public int[] GetValuesByValueType(ValueType type)
	{
		if (type == UnitValueType.MaxHp || type == UnitValueType.Hp)
		{
			return new int[] { MaxHp };
		}
		else if (type == UnitValueType.Attack)
		{
			return new int[] { Attack };
		}

		return new int[]{};
	}

	public void SetValuesByValueType(ValueType type, int[] newValues)
	{
		if (type == UnitValueType.MaxHp || type == UnitValueType.Hp)
		{
			MaxHp = newValues[0];
		}
		else if (type == UnitValueType.Attack)
		{
			Attack = newValues[0];
		}
	}
}