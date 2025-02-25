
using MessageSystem;

public class BattleObjectTypeEliminateNotice : Notice
{
	public ObjectType Type { get; }
	public IUpdatableRoutine Context { get; }
	public BattleObjectTypeEliminateNotice(ObjectType targetType, IUpdatableRoutine context)
	{
		Context = context;
		Type = targetType;
	}
}