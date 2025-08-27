using System.Collections.Generic;

public class UnitSkillCardStatSpec : SkillCardStatSpec
{
	public static UnitSkillCardStatSpec Create(Dictionary<string, object> param)
	{
		//string -> enum -> dictionary util로 값 받기	
		var spec = new UnitSkillCardStatSpec();
		spec.Name = param.GetString(nameof(Name));
		spec.ValueDict = new();
		foreach (var kvp in param)
		{
			if (ValueType.TryParse(kvp.Key, out ValueType type))
			{
				spec.ValueDict[type] = param.GetIntArray(kvp.Key);
			}
		}
		
		return spec;
	}
}