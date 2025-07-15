public enum ValueType
{
	None,
	MaxHp,
	Hp,
	TurnCount,
	MaxTurnCount,
	Attack,
	Cost,
	Shield
}

public interface IStat
{
	public int[] GetValuesByValueType(ValueType type);
}

public static class IStatExtensions
{
	public static int GetValueByValueType(this IStat stat, ValueType type)
	{
		var values = stat.GetValuesByValueType(type);
		return values == null || values.Length == 0 ? -1 : values[0];
	}
}