using System.Collections.Generic;
using Newtonsoft.Json;

public class StatData : GameData
{
	private Dictionary<string, UnitStatSpec> unitStatDict;
	private Dictionary<string, SkillStatSpec> skillStatDict;

	public UnitStatSpec GetUnitStatByName(string name)
	{
		return unitStatDict.GetValueOrDefault(name);
	}
	
	public SkillStatSpec GetSkillStatByName(string name)
	{
		return skillStatDict.GetValueOrDefault(name);
	}

	
	public override void Initialize()
	{
		unitStatDict = new();
		var deserializedObject = GameDataSystem.Instance.GameDataParams["UnitCardStatData"];
		foreach (var specJson in deserializedObject)
		{
			var stat = UnitStatSpec.Create(specJson);
			unitStatDict[stat.Name] = stat;
		}
		
		
		skillStatDict = new();
		deserializedObject = GameDataSystem.Instance.GameDataParams["SkillCardStatData"];
		foreach (var specJson in deserializedObject)
		{
			var stat = SkillStatSpec.Create(specJson);
			skillStatDict[stat.Name] = stat;
		}
	}

	public override void Dispose()
	{
		throw new System.NotImplementedException();
	}
}