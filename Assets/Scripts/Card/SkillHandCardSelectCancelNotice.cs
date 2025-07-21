
using MessageSystem;

public class SkillHandCardSelectCancelNotice : Notice
{
	public SkillCardInHand SelectedCard { get; }

	public SkillHandCardSelectCancelNotice(SkillCardInHand card)
	{
		SelectedCard = card;
	}
}