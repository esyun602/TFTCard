
public class UnitSkillCard : SkillCard
{
	private SkillCard targetSkillCard;
	public IBattleObject Owner { get; set; }

	public UnitSkillCard(SkillCard targetSkillCard) : base(targetSkillCard.SkillCardStaticSpec)
	{
	}
}