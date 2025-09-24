public static class ValueTypeExtensions
{
	public static bool IsSkillCompatible(this ValueType type)
	{
		return type is CommonValueType or SkillValueType;
	}

	public static bool IsUnitCompatible(this ValueType type)
	{
		return type is CommonValueType or UnitValueType;
	}
}