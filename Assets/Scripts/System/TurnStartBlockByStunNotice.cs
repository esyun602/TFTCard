using MessageSystem;

public class TurnStartBlockByStunNotice : Notice
{
	public ITurnObject TargetObject { get; }

	public TurnStartBlockByStunNotice(ITurnObject target)
	{
		TargetObject = target;	
	}
}