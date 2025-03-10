using MessageSystem;

public class PlayerTurnEndNotice : Notice
{
	public PlayerTurn PlayerTurnObject { get; }

	public PlayerTurnEndNotice(PlayerTurn obj)
	{
		PlayerTurnObject = obj;
	}
}