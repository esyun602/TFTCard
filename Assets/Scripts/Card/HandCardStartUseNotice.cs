
using MessageSystem;

public class HandCardStartUseNotice : Notice
{
	public BattleCardObjectInHand SelectedCard { get; }

	public HandCardStartUseNotice(BattleCardObjectInHand card)
	{
		SelectedCard = card;
	}
}