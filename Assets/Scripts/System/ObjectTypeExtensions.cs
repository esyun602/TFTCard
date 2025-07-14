public static class ObjectTypeExtensions
{
	public static bool IsHostile(this ObjectType type, ObjectType target)
	{
		return type == ObjectType.Ally && target == ObjectType.Enemy ||
		       type == ObjectType.Enemy && target == ObjectType.Ally;
	}
	
	public static ObjectType GetOpposite(this ObjectType type)
	{
		return type switch
		{
			ObjectType.Ally => ObjectType.Enemy,
			ObjectType.Enemy => ObjectType.Ally,
			_ => ObjectType.Neutral
		};
	}
}
