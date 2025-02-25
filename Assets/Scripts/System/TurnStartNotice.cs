using MessageSystem;

public class TurnStartNotice : Notice
{
	public ITurnObject TargetObject { get; }

	public TurnStartNotice(ITurnObject target)
	{
		TargetObject = target;
	}
}