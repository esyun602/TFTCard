using System.Collections.Generic;
using MessageSystem;
using UnityEngine;

public class SkillCardBattleStat : IStat
{	
	private SkillCardStat originStat;
	
	public int TurnCountValue { get; set; }
	public int HpValue { get; set; }
	public int AttackValue { get; set; }
	public int CostValue { get; set; }

	private int turnCount;
	private List<IOption> optionList;
	//field scope 기믹
	private List<IBuff> buffList;
	private List<Synergy> synergyList;

	public SkillCardBattleStat(SkillCardStat skillCardStat)
	{
		originStat = skillCardStat;
		AttackValue = skillCardStat.AttackValue;
		HpValue = skillCardStat.HpValue;
		turnCount = TurnCountValue;
		CostValue = skillCardStat.CostValue;
	}
	
	public int[] GetValuesByValueType(ValueType type)
	{
		switch (type)
		{
			case ValueType.MaxHp:
			case ValueType.Hp:
				return new int[] { HpValue };
			case ValueType.TurnCount:
			case ValueType.MaxTurnCount:
				return new int[] { TurnCountValue };
			case ValueType.Attack:
				return new int[] { AttackValue };
			case ValueType.Cost:
				return new int[] { CostValue };
			default:
				return new int[] { };
		}
	}
}