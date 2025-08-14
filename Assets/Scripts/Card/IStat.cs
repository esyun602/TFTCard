public enum BattleValueType
{
	None,
	MaxHp,
	Hp,
	TurnCount,
	MaxTurnCount,
	Attack,
	Cost,
	Shield,
	Burn,
	Catalyst,
	Stun,
	Dodge,
	HealBan,
	Draw
}

public interface IStat
{
	public int[] GetValuesByValueType(BattleValueType type);
	public void SetValuesByValueType(BattleValueType type, int[] newValues);
}

public static class IStatExtensions
{
	public static int GetValueByValueType(this IStat stat, BattleValueType type)
	{
		var values = stat.GetValuesByValueType(type);
		return values == null || values.Length == 0 ? 0 : values[0];
	}

	public static void SetValueByValueType(this IStat stat, BattleValueType type, int value)
	{
		stat.SetValuesByValueType(type, new int[] { value });
	}

	public static void AddValueByValueType(this IStat stat, BattleValueType type, int value)
	{
		var originValues = stat.GetValuesByValueType(type);
		for (var i = 0; i < originValues.Length; i++)
		{
			originValues[i] += value;
		}
		stat.SetValuesByValueType(type, originValues);
	}
}