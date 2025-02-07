using MessageSystem;

public class HandCardSelectNotice : Notice
{
	public BattleCardObjectInHand SelectedCard { get; }

	public HandCardSelectNotice(BattleCardObjectInHand card)
	{
		SelectedCard = card;
	}
}