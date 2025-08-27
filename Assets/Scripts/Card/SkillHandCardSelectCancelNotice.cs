
using MessageSystem;

public class SkillHandCardSelectCancelNotice : Notice
{
	public BattleCardObjectInHand SelectedCard { get; }

	public SkillHandCardSelectCancelNotice(BattleCardObjectInHand card)
	{
		SelectedCard = card;
	}
}