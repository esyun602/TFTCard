using System;
using System.Collections.Generic;
using UnityEngine;


//todo: dictionary 형태 인스펙터에 띄우기
public class UnitStatSpec : IStat
{
	public string Name { get; private set; }
	public Dictionary<ValueType, int[]> ValueDict { get; private set; }
	public List<SynergyCategory> SynergyList { get; private set; }

	private UnitStatSpec()
	{
	}

	public static UnitStatSpec Create(Dictionary<string, object> param)
	{
		//string -> enum -> dictionary util로 값 받기	
		var spec = new UnitStatSpec();
		spec.Name = param.GetString(nameof(Name));
		spec.ValueDict = new();
		foreach (var kvp in param)
		{
			if (ValueType.TryParse(kvp.Key, out ValueType type))
			{
				var array = param.GetIntArray(kvp.Key);
				if (array != null)
				{
					spec.ValueDict[type] = array;
				}
				else
				{
					spec.ValueDict[type] = new int[] { param.GetInt(kvp.Key) };
				}
			}
		}

		spec.SynergyList = new();
		foreach (var str in param.GetStringArray(nameof(SynergyList)))
		{
			if (Enum.TryParse(str, out SynergyCategory synergy))
			{
				spec.SynergyList.Add(synergy);
			}
			else
			{
				// throw new InvalidOperationException();
			}
		}
		
		spec.OnInitialize(param);
		
		return spec;
	}

	protected virtual void OnInitialize(Dictionary<string, object> param)
	{
		
	}

	public int[] GetValuesByValueType(ValueType type)
	{
		return ValueDict.GetValueOrDefault(type, new int[] { });
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