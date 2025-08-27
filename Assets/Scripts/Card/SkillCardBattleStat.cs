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
		return valueDict.GetValueOrDefault(type);
	}

	public virtual void SetValuesByValueType(ValueType type, int[] newValues)
	{
		if (!type.IsSkillCompatible()) throw new ArgumentException();
		valueDict[type] = newValues;
	}
}