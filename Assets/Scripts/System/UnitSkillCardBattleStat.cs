public class UnitSkillCardBattleStat : SkillCardBattleStat
{
	public IBattleObject Owner { get; set; }
	public UnitSkillCardStat OriginStat { get; }
	
	//todo: fix?
	public UnitSkillCardBattleStat(UnitSkillCardStat skillCardStat, IBattleObject owner) : base(skillCardStat)
	{
		Owner = owner;
		OriginStat = skillCardStat;
	}
	
	public override int[] GetValuesByValueType(ValueType type)
	{
		//todo: 그냥 valuetype일 때 처리
		if (type is UnitValueType)
		{
			if (Owner == null)
			{
				return new[] { 0 };
			}
			return Owner.UnitCardBattleStat.GetValuesByValueType(type);
		}

		return base.GetValuesByValueType(type);
	}
}