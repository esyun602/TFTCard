using MessageSystem;

public class SkillHandCardSelectNotice : Notice
{
	public BattleCardObjectInHand SelectedCard { get; }

	public SkillHandCardSelectNotice(BattleCardObjectInHand card)
	{
		SelectedCard = card;
	}
}