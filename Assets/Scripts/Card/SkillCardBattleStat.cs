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
		if (Owner == null) return valueDict.GetValueOrDefault(type, new int[]{});
		
		var ownerValues = Owner.UnitCardBattleStat.GetValuesByValueType(type);
		var ownValues = valueDict.GetValueOrDefault(type, new int[] { });
		
		var newArray = new int[Mathf.Max(ownerValues.Length, ownValues.Length)];

		for (var i = 0; i < newArray.Length; i++)
		{
			var val = 0;
			if (ownerValues.Length > i)
			{
				val += ownerValues[i];
			}

			if (ownValues.Length > i)
			{
				val += ownValues[i];
			}

			newArray[i] = val;
		}

		return newArray;
	}

	public void SetValuesByValueType(BattleValueType type, int[] newValues)
	{
		if(Owner == null) valueDict[type] = newValues;
		else
		{
			var ownerValues = Owner.UnitCardBattleStat.GetValuesByValueType(type);

			for (var i = 0; i < newValues.Length; i++)
			{
				newValues[i] -= ownerValues[i];
			}
			
			valueDict[type] = newValues;
		}
	}
}