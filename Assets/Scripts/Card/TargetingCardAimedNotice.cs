using MessageSystem;

public class TargetingCardAimedNotice : Notice
{
	public BattleCardObjectInHand Card { get; }

	public TargetingCardAimedNotice(BattleCardObjectInHand card)
	{
		Card = card;
	}
	
}