using System;

public static class TacticsValueTypeExtensions
{
	public static bool Contains(this TacticsValueType origin, TacticsValueType target)
	{
		return (origin & target) == target;
	}
}