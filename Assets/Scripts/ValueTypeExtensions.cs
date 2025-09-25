public static class ValueTypeExtensions
{
	public static bool IsSkillCompatible(this ValueType type)
	{
		return type is SkillValueType;
	}

	public static bool IsUnitCompatible(this ValueType type)
	{
		return type is SkillValueType or UnitValueType;
	}
}