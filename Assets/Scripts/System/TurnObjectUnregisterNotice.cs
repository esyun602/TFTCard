using MessageSystem;

public class TurnObjectUnregisterNotice : Notice
{
	public ITurnObject TurnObject { get; }

	public TurnObjectUnregisterNotice(ITurnObject turnObject)
	{
		this.TurnObject = turnObject;
	}
}