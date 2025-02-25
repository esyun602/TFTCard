using MessageSystem;

public class TurnObjectDestroyNotice : Notice
{
	public TurnObjectDestroyNotice(ITurnObject target)
	{
		Target = target;
	}

	public ITurnObject Target { get; }
}