using MessageSystem;

public class PlayerTurnEndNotice : Notice
{
	public ITurnObject PlayerTurnObject { get; }

	public PlayerTurnEndNotice(ITurnObject obj)
	{
		PlayerTurnObject = obj;
	}
}