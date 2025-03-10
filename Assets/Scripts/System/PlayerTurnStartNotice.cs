using MessageSystem;

public class PlayerTurnStartNotice : Notice
{
	public PlayerTurn PlayerTurnObject { get; }

	public PlayerTurnStartNotice(PlayerTurn obj)
	{
		PlayerTurnObject = obj;
	}
}