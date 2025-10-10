using System.Linq;

public class UnitSkillCardBattleStat : SkillCardBattleStat
{
	public IBattleObject Owner { get; }
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

		var unitValue = Owner?.UnitCardBattleStat.GetValueByValueType(type) ?? 0;
		var baseValue = base.GetValuesByValueType(type);
		var ret = new int[baseValue.Length];
		for (var i = 0; i < ret.Length; i++)
		{
			ret[i] = baseValue[i] + unitValue;
		}

		return ret;
	}
}