
using MessageSystem;

public class HandCardSelectCancelNotice : Notice
{
	public BattleCardObjectInHand SelectedCard { get; }

	public HandCardSelectCancelNotice(BattleCardObjectInHand card)
	{
		SelectedCard = card;
	}
}