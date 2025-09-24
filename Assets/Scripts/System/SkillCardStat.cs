//todo: type에 따른 dictionary 방식으로 수정하는게 나을듯
//todo: skill카드에서 value set 관련 수정

using System;
using System.Collections.Generic;

public abstract class SkillCardStat : IStat
{
	public Dictionary<ValueType, int[]> ValueDict { get; }
	protected SkillCardStat(SkillCardStatSpec cardStatSpec)
	{
		ValueDict = new(cardStatSpec.ValueDict);
	}

	public virtual int[] GetValuesByValueType(ValueType type)
	{
		if (!type.IsSkillCompatible()) throw new ArgumentException();
		return ValueDict.GetValueOrDefault(type);
	}

	public virtual void SetValuesByValueType(ValueType type, int[] newValues)
	{
		if (!type.IsSkillCompatible()) throw new ArgumentException();
		ValueDict[type] = newValues;
	}
}