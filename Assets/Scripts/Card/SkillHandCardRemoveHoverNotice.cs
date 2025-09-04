using MessageSystem;

public class SkillHandCardRemoveHoverNotice : Notice
{
	public BattleCardObjectInHand SelectedCard { get; }

	public SkillHandCardRemoveHoverNotice(BattleCardObjectInHand card)
	{
		SelectedCard = card;
	}
}