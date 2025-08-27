public class TacticsCard : SkillCardBase
{
	public TacticsCard(TacticsCardSpec spec) : base(spec)
	{
		var statSpec = GameDataSystem.Instance.GetGameData<StatData>().GetTacticsStatByName(spec.StatSpecName);
		Stat = new TacticsCardStat(statSpec);
	}
}