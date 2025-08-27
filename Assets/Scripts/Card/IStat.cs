public interface IStat
{
	public int[] GetValuesByValueType(ValueType type);
	public void SetValuesByValueType(ValueType type, int[] newValues);
}

public static class IStatExtensions
{
	public static int GetValueByValueType(this IStat stat, ValueType type)
	{
		var values = stat.GetValuesByValueType(type);
		return values == null || values.Length == 0 ? 0 : values[0];
	}

	public static void SetValueByValueType(this IStat stat, ValueType type, int value)
	{
		stat.SetValuesByValueType(type, new int[] { value });
	}

	public static void AddValueByValueType(this IStat stat, ValueType type, int value)
	{
		var originValues = stat.GetValuesByValueType(type);
		for (var i = 0; i < originValues.Length; i++)
		{
			originValues[i] += value;
		}

		stat.SetValuesByValueType(type, originValues);
	}
}