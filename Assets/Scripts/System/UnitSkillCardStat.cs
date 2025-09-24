public class UnitSkillCardStat : SkillCardStat
{
	public UnitCard Owner { get; }

	public UnitSkillCardStat(SkillCardStatSpec cardStatSpec, UnitCard owner) : base(cardStatSpec)
	{
		Owner = owner;
	}

	public override int[] GetValuesByValueType(ValueType type)
	{
		//todo: 그냥 valuetype일 때 처리
		if (type is UnitValueType) return Owner.Stat.GetValuesByValueType(type);

		return base.GetValuesByValueType(type);
	}
}