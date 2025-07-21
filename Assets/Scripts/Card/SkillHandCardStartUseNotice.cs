
using MessageSystem;

public class SkillHandCardStartUseNotice : Notice
{
	public SkillCardInHand SelectedCard { get; }

	public SkillHandCardStartUseNotice(SkillCardInHand card)
	{
		SelectedCard = card;
	}
}