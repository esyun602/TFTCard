
using MessageSystem;

public class SkillHandCardEndUseNotice : Notice
{
	public BattleCardObjectInHand SelectedCard { get; }

	public SkillHandCardEndUseNotice(BattleCardObjectInHand card)
	{
		SelectedCard = card;
	}
}