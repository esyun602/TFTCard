using System;
using System.Collections.Generic;

[Flags]
public enum TacticsValueType
{
	None = 0,
	Exhaustion = 1,
}

public class SkillStatSpec : IStat
{
	public string Name { get; private set; }
	public Dictionary<BattleValueType, int[]> ValueDict { get; private set; }
	public TacticsValueType TacticsValue { get; private set; }
	private SkillStatSpec()
	{
		
	}
	
	public static SkillStatSpec Create(Dictionary<string, object> param)
	{
		//string -> enum -> dictionary util로 값 받기	
		var spec = new SkillStatSpec();
		spec.Name = param.GetString(nameof(Name));
		spec.ValueDict = new();
		spec.TacticsValue = TacticsValueType.None;
		foreach (var kvp in param)
		{
			if (Enum.TryParse(kvp.Key, out BattleValueType type))
			{
				spec.ValueDict[type] = param.GetIntArray(kvp.Key);
			}
			else if (Enum.TryParse(kvp.Key, out TacticsValueType tacticsValueType))
			{
				spec.TacticsValue |= tacticsValueType;
			}
		}
		
		return spec;
	}
	
	public int[] GetValuesByValueType(BattleValueType type)
	{
		return ValueDict.GetValueOrDefault(type, new int[]{});
	}

	/// <summary>
	/// 스펙에 값을 덮어씌우는 건 막기
	/// </summary>
	/// <param name="type"></param>
	/// <param name="newValues"></param>
	public void SetValuesByValueType(BattleValueType type, int[] newValues)
	{
#if UNITY_EDITOR
		throw new ArgumentException();
#endif
	}
}