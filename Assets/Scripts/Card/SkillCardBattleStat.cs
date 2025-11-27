using System;
using System.Collections.Generic;
using MessageSystem;
using UnityEngine;

//todo: 추후에 데이터 빼면 hashmap으로 묶어 쓰는게 나을 듯
//todo: owner 있는 타입을 분리해서?
public abstract class SkillCardBattleStat : IStat
{
	private Dictionary<ValueType, int[]> valueDict;

	public SkillCardBattleStat(SkillCardStat skillCardStat)
	{
		valueDict = new(skillCardStat.ValueDict);
	}

	public virtual int[] GetValuesByValueType(ValueType type)
	{
		if (!type.IsSkillCompatible()) throw new ArgumentException();
		//todo: instance 하나 재활용
		return valueDict.GetValueOrDefault(type, new int[]{0});
	}

	public virtual void SetValuesByValueType(ValueType type, int[] newValues)
	{
		if (!type.IsSkillCompatible()) throw new ArgumentException();
		valueDict[type] = newValues;
	}
}

public static class SkillCardBattleStatExtensions
{
	public static int GetCostValueWithModifier(this SkillCardBattleStat stat)
	{
		return Mathf.Max(0, stat.GetValueByValueType(SkillValueType.Cost) +
		                    (Game.Instance.GetGameMode<BattleStageGameMode>()?.BattleGlobalModifier.CardCostAdd ?? 0));
	}
}