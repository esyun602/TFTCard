using System.Collections.Generic;
using MessageSystem;
using UnityEngine;

//todo: 추후에 데이터 빼면 hashmap으로 묶어 쓰는게 나을 듯
//todo: owner 있는 타입을 분리해서?
public class SkillCardBattleStat : IStat
{
	private SkillCardStat originStat;
	public IBattleObject Owner { get; set; }
	
	public int TurnCountValue { get; set; }
	public int HpValue { get; set; }
	public int AttackValue { get; set; }
	public int CostValue { get; set; }
	public int ShieldValue { get; set; }
	public int BurnValue { get; set; }
	public int CatalystValue { get; set; }
	public int StunValue { get; set; }
	public int DodgeValue { get; set; }
	

	public SkillCardBattleStat(SkillCardStat skillCardStat)
	{
		originStat = skillCardStat;
		AttackValue = skillCardStat.AttackValue;
		HpValue = skillCardStat.HpValue;
		TurnCountValue = skillCardStat.TurnCountValue;
		CostValue = skillCardStat.CostValue;
		ShieldValue = skillCardStat.ShieldValue;
		
		BurnValue = skillCardStat.BurnValue;
		CatalystValue = skillCardStat.CatalystValue;
		StunValue = skillCardStat.StunValue;
		DodgeValue = skillCardStat.DodgeValue;
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
			case ValueType.Shield:
				return new int[] { ShieldValue };
			case ValueType.Burn:
				return new int[] { BurnValue };
			case ValueType.Catalyst:
				return new int[] { CatalystValue };
			case ValueType.Stun:
				return new int[] { StunValue };
			case ValueType.Dodge:
				return new int[] { DodgeValue };
			default:
				return new int[] { };
		}
	}
}