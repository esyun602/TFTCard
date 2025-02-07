using MessageSystem;

public class PlayerTurnStartNotice : Notice
{
	public ITurnObject PlayerTurnObject { get; }

	public PlayerTurnStartNotice(ITurnObject obj)
	{
		PlayerTurnObject = obj;
	}
}