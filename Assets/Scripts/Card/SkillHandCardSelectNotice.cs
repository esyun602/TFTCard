using MessageSystem;

public class SkillHandCardSelectNotice : Notice
{
	public SkillCardInHand SelectedCard { get; }

	public SkillHandCardSelectNotice(SkillCardInHand card)
	{
		SelectedCard = card;
	}
}