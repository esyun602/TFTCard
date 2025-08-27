public class UnitSkillCard : SkillCardBase
{
	public UnitSkillCardStat UnitSkillCardStat => (UnitSkillCardStat)Stat;
	public UnitSkillCard(UnitSkillCardSpec spec, UnitCard owner) : base(spec)
	{
		var statSpec = GameDataSystem.Instance.GetGameData<StatData>().GetUnitSkillStatByName(spec.StatSpecName);
		Stat = new UnitSkillCardStat(statSpec, owner);
	}
}