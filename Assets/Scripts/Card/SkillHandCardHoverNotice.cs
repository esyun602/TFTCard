using MessageSystem;

public class SkillHandCardHoverNotice : Notice
{
	public BattleCardObjectInHand SelectedCard { get; }

	public SkillHandCardHoverNotice(BattleCardObjectInHand card)
	{
		SelectedCard = card;
	}
}