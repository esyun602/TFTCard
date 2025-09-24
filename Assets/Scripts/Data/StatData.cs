using System.Collections.Generic;
using Newtonsoft.Json;

public class StatData : GameData
{
	private Dictionary<string, UnitStatSpec> unitStatDict;
	private Dictionary<string, TacticsCardStatSpec> tacticsStatDict;
	private Dictionary<string, UnitSkillCardStatSpec> unitSkillStatDict;
	

	public UnitStatSpec GetUnitStatByName(string name)
	{
		return unitStatDict.GetValueOrDefault(name);
	}
	
	public SkillCardStatSpec GetTacticsStatByName(string name)
	{
		return tacticsStatDict.GetValueOrDefault(name);
	}
	
	public SkillCardStatSpec GetUnitSkillStatByName(string name)
	{
		return unitSkillStatDict.GetValueOrDefault(name);
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
		
		
		tacticsStatDict = new();
		deserializedObject = GameDataSystem.Instance.GameDataParams["TacticsCardStatData"];
		foreach (var specJson in deserializedObject)
		{
			var stat = TacticsCardStatSpec.Create(specJson);
			tacticsStatDict[stat.Name] = stat;
		}
				
		unitSkillStatDict = new();
		deserializedObject = GameDataSystem.Instance.GameDataParams["UnitSkillCardStatData"];
		foreach (var specJson in deserializedObject)
		{
			var stat = UnitSkillCardStatSpec.Create(specJson);
			unitSkillStatDict[stat.Name] = stat;
		}
	}

	public override void Dispose()
	{
		throw new System.NotImplementedException();
	}
}