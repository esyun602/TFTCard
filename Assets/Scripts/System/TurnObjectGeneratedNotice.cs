using MessageSystem;

public class TurnObjectGeneratedNotice : Notice
{
	public TurnObjectGeneratedNotice(ITurnObject target)
	{
		Target = target;
	}

	public ITurnObject Target { get; }
}