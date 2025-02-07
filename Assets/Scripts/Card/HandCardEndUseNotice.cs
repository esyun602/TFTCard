
using MessageSystem;

public class HandCardEndUseNotice : Notice
{
	public BattleCardObjectInHand SelectedCard { get; }

	public HandCardEndUseNotice(BattleCardObjectInHand card)
	{
		SelectedCard = card;
	}
}