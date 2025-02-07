
using MessageSystem;

public class TurnObjectRegisterNotice : Notice
{
	public ITurnObject TurnObject { get; }

	public TurnObjectRegisterNotice(ITurnObject turnObject)
	{
		this.TurnObject = turnObject;
	}
}