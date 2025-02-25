
using MessageSystem;

public class PlayerFieldCardMoveNotice : Notice
{
	public PlayerFieldCardMoveNotice(IBattleObject target)
	{
		Target = target;
	}

	public IBattleObject Target { get; }
}