using MessageSystem;

public class CardActionTriggerNotice : Notice
{
	public IBattleObject Owner { get; }
	public IAction targetAction { get; }

	public CardActionTriggerNotice(IBattleObject owner, IAction triggeredAction)
	{
		Owner = owner;
		targetAction = triggeredAction;
	}
}