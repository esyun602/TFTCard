
using MessageSystem;

public class SkillHandCardEndUseNotice : Notice
{
	public SkillCardInHand SelectedCard { get; }

	public SkillHandCardEndUseNotice(SkillCardInHand card)
	{
		SelectedCard = card;
	}
}