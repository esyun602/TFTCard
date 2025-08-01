public class SkillCard : ICard
{
	public SkillCardStat Stat { get; }
	public SkillCardActionBase Action { get; }
	public SkillCardSpec SkillCardStaticSpec { get; }
	

	public SkillCard(SkillCardSpec spec)
	{
		SkillCardStaticSpec = spec;
		Action = spec.actionSpec.CreateCardAction();
		Stat = new SkillCardStat(spec.statSpec);
	}

	public ICardSpec CardStaticSpec => SkillCardStaticSpec;
}