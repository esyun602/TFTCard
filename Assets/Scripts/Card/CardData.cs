using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class CardData : GameData
{
	private List<UnitCardSpec> unitCardSpecList;
	private List<UnitSkillCardSpec> unitSkillCardSpecList;
	private List<TacticsCardSpec> tacticsCardSpecList;
	
	public override void Initialize()
	{
		unitCardSpecList = new();
		var deserializedObject = GameDataSystem.Instance.GameDataParams["UnitCardData"];
		foreach (var specJson in deserializedObject)
		{
			unitCardSpecList.Add(UnitCardSpec.Create(specJson));
		}
		
		
		unitSkillCardSpecList = new();
		deserializedObject = GameDataSystem.Instance.GameDataParams["UnitSkillCardData"];
		foreach (var specJson in deserializedObject)
		{
			unitSkillCardSpecList.Add(UnitSkillCardSpec.Create(specJson));
		}
		
		tacticsCardSpecList = new();
		deserializedObject = GameDataSystem.Instance.GameDataParams["TacticsCardData"];
		foreach (var specJson in deserializedObject)
		{
			tacticsCardSpecList.Add(TacticsCardSpec.Create(specJson));
		}
	}

	public override void Dispose()
	{
	}

	public ICardSpec GetRandomUnitCardSpec()
	{
		return unitCardSpecList[Random.Range(0, unitCardSpecList.Count)];
	}
	

	public TacticsCardSpec GetRandomTacticsCardSpec(bool includeUnitAction = false)
	{
		return tacticsCardSpecList.GetRandomElement();
	}
	
	public List<TacticsCardSpec> GetRandomTacticsCardSpecs(int count, bool includeUnitAction = false)
	{
		var ret = new List<TacticsCardSpec>();
		for (var i = 0; i < count; i++)
		{
			var randomIdx = Random.Range(0, tacticsCardSpecList.Count);
			while (ret.Contains(tacticsCardSpecList[randomIdx]))
			{
				randomIdx = Random.Range(0, tacticsCardSpecList.Count);
			}
			
			ret.Add(tacticsCardSpecList[randomIdx]);
		}
		return ret;
	}
	
	//todo: fix?
	public ICardSpec GetSpecById(int id)
	{
		return unitCardSpecList[id];
	}

	//todo: 별도로 dit 만들어서
	public TacticsCardSpec GetTacticsCardSpecByName(string str)
	{
		return tacticsCardSpecList.Find(x => x.Name == str);
	}
	
	public UnitSkillCardSpec GetUnitSkillCardSpecByName(string str)
	{
		return unitSkillCardSpecList.Find(x => x.Name == str);
	}
	
	//todo: 별도로 dit 만들어서
	public UnitCardSpec GetUnitCardSpecByName(string str)
	{
		return unitCardSpecList.Find(x => x.Name == str);
	}

	//todo: fix
	public UnitCardSpec GetUnitCardSpecById(int id)
	{
		return unitCardSpecList[id];
		
	}
	
	public SkillCardSpec GetSkillCardSpecById(int id)
	{
		return unitSkillCardSpecList[id];
		
	}
}