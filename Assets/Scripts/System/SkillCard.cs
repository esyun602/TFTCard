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
	public string Name => GameDataSystem.Instance.GetGameData<GameString>().GetString(CardStaticSpec.NameKey);
	//todo: 설명은 액션으로 ?
	public string Desc => GameDataSystem.Instance.GetGameData<GameString>()
		.Format(CardStaticSpec.DescKey, Action.DescParams);
}