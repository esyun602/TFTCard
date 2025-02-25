using MessageSystem;

public class TurnEndNotice : Notice
{
	public ITurnObject TargetObject { get; }

	public TurnEndNotice(ITurnObject target)
	{
		TargetObject = target;
	}
}