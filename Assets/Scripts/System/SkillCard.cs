public class SkillCard : ICard
{
	public SkillCardStat Stat { get; }
	public SkillCardActionBase Action { get; }
	public SkillCardSpec SkillCardStaticSpec { get; }
	

	public SkillCard(SkillCardSpec spec)
	{
		SkillCardStaticSpec = spec;
		
		var statSpec = GameDataSystem.Instance.GetGameData<StatData>().GetSkillStatByName(spec.StatSpecName);
		Stat = new SkillCardStat(statSpec);
		var actionSpec = GameDataSystem.Instance.GetGameData<ActionData>().GetSkillActionByName(spec.ActionSpecName);
		Action = actionSpec.CreateCardAction();
		Action.SetCardStat(Stat);
	}
	
	//todo: fix later
	public UnitCard Owner { get; }
	public SkillCard(SkillCardSpec spec, UnitCard owner)
	{
		Owner = owner;
		
		SkillCardStaticSpec = spec;
		
		var statSpec = GameDataSystem.Instance.GetGameData<StatData>().GetSkillStatByName(spec.StatSpecName);
		Stat = new SkillCardStat(statSpec);
		var actionSpec = GameDataSystem.Instance.GetGameData<ActionData>().GetSkillActionByName(spec.ActionSpecName);
		Action = actionSpec.CreateCardAction();
		Action.SetCardStat(Stat);
	}
	
	public ICardSpec CardStaticSpec => SkillCardStaticSpec;
	public string Name => GameDataSystem.Instance.GetGameData<GameString>().GetString(CardStaticSpec.NameKey);
	//todo: 설명은 액션으로 ?
	public string Desc => GameDataSystem.Instance.GetGameData<GameString>()
		.Format(CardStaticSpec.DescKey, Action.DescParams);
}