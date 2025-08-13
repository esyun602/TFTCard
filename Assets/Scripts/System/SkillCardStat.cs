//todo: type에 따른 dictionary 방식으로 수정하는게 나을듯
//todo: skill카드에서 value set 관련 수정

using System.Collections.Generic;

public class SkillCardStat : IStat
{
	public Dictionary<BattleValueType, int[]> ValueDict { get; private set; }
	public TacticsValueType TacticsValueType { get; set; }
	public SkillCardStat(SkillStatSpec statSpec)
	{
		ValueDict = new(statSpec.ValueDict);
		TacticsValueType = statSpec.TacticsValue;
	}

	public int[] GetValuesByValueType(BattleValueType type)
	{
		return ValueDict.GetValueOrDefault(type, new int[]{});
	}

	public void SetValuesByValueType(BattleValueType type, int[] newValues)
	{
		ValueDict[type] = newValues;
	}
}