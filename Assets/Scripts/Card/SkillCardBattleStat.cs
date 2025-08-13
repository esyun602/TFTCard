using System.Collections.Generic;
using MessageSystem;
using UnityEngine;

//todo: 추후에 데이터 빼면 hashmap으로 묶어 쓰는게 나을 듯
//todo: owner 있는 타입을 분리해서?
public class SkillCardBattleStat : IStat
{
	private SkillCardStat originStat;
	public IBattleObject Owner { get; set; }
	
	private Dictionary<BattleValueType, int[]> valueDict;
	public TacticsValueType TacticsValueType { get; set; }

	public SkillCardBattleStat(SkillCardStat skillCardStat)
	{
		originStat = skillCardStat;
		valueDict = new(skillCardStat.ValueDict);
		TacticsValueType = skillCardStat.TacticsValueType;
	}
	
	public int[] GetValuesByValueType(BattleValueType type)
	{
		return valueDict.GetValueOrDefault(type, new int[]{});
	}

	public void SetValuesByValueType(BattleValueType type, int[] newValues)
	{
		valueDict[type] = newValues;
	}
}