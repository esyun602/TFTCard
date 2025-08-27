public abstract class SkillCardBase : ICard
{
	private SkillCardStat stat;
	public SkillCardStat Stat
	{
		get => stat;
		protected set
		{
			stat = value;
			Action.SetCardStat(Stat);
		}
	}

	public SkillCardActionBase Action { get; }
	public SkillCardSpec SkillCardStaticSpec { get; }
	

	public SkillCardBase(SkillCardSpec spec)
	{
		SkillCardStaticSpec = spec;
		
		var actionSpec = GameDataSystem.Instance.GetGameData<ActionData>().GetSkillActionByName(spec.ActionSpecName);
		Action = actionSpec.CreateCardAction();
	}
	
	public ICardSpec CardStaticSpec => SkillCardStaticSpec;
	public string Name => GameDataSystem.Instance.GetGameData<GameString>().GetString(CardStaticSpec.NameKey);
	//todo: 설명은 액션으로 ?
	public string Desc => GameDataSystem.Instance.GetGameData<GameString>()
		.Format(CardStaticSpec.DescKey, Action.DescParams);
}