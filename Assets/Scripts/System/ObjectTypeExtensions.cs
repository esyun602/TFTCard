public static class ObjectTypeExtensions
{
	public static bool IsHostile(this ObjectType type, ObjectType target)
	{
		return type == ObjectType.Ally && target == ObjectType.Enemy ||
		       type == ObjectType.Enemy && target == ObjectType.Ally;
	}
}