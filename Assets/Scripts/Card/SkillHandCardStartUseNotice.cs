
using MessageSystem;

public class SkillHandCardStartUseNotice : Notice
{
	public BattleCardObjectInHand SelectedCard { get; }

	public SkillHandCardStartUseNotice(BattleCardObjectInHand card)
	{
		SelectedCard = card;
	}
}