using System;
using System.Collections.Generic;

public abstract class SkillCardStatSpec : IStat
{
	public string Name { get; protected set; }
	public Dictionary<ValueType, int[]> ValueDict { get; protected set; }
	protected SkillCardStatSpec()
	{
		
	}
	
	public int[] GetValuesByValueType(ValueType type)
	{
		return ValueDict.GetValueOrDefault(type, new int[]{});
	}

	/// <summary>
	/// 스펙에 값을 덮어씌우는 건 막기
	/// </summary>
	/// <param name="type"></param>
	/// <param name="newValues"></param>
	public void SetValuesByValueType(ValueType type, int[] newValues)
	{
#if UNITY_EDITOR
		throw new ArgumentException();
#endif
	}
}