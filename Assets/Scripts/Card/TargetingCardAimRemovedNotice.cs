using MessageSystem;

public class TargetingCardAimRemovedNotice : Notice
{
	public BattleCardObjectInHand Card { get; }

	public TargetingCardAimRemovedNotice(BattleCardObjectInHand card)
	{
		Card = card;
	}
}