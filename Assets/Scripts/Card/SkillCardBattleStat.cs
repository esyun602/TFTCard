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
	//todo:fix
	public bool IsExhaustion { get; set; }
	

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
		
		//todo:fix
		IsExhaustion = skillCardStat.IsExhaustion;
	}
	
	public int[] GetValuesByValueType(BattleValueType type)
	{
		switch (type)
		{
			case BattleValueType.MaxHp:
			case BattleValueType.Hp:
				return new int[] { HpValue };
			case BattleValueType.TurnCount:
			case BattleValueType.MaxTurnCount:
				return new int[] { TurnCountValue };
			case BattleValueType.Attack:
				return new int[] { AttackValue };
			case BattleValueType.Cost:
				return new int[] { CostValue };
			case BattleValueType.Shield:
				return new int[] { ShieldValue };
			case BattleValueType.Burn:
				return new int[] { BurnValue };
			case BattleValueType.Catalyst:
				return new int[] { CatalystValue };
			case BattleValueType.Stun:
				return new int[] { StunValue };
			case BattleValueType.Dodge:
				return new int[] { DodgeValue };
			default:
				return new int[] { };
		}
	}
}