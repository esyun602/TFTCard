using MessageSystem;

public class CardActionEndNotice : Notice
{
	public IBattleObject Owner { get; }
	public IAction targetAction { get; }

	public CardActionEndNotice(IBattleObject owner, IAction triggeredAction)
	{
		Owner = owner;
		targetAction = triggeredAction;
	}
}