using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class CardData : GameData
{
	private List<UnitCardSpec> unitCardSpecList;
	private List<SkillCardSpec> skillCardSpecList;
	
	public override void Initialize()
	{
		unitCardSpecList = new();
		var deserializedObject = GameDataSystem.Instance.GameDataParams["UnitCardData"];
		foreach (var specJson in deserializedObject)
		{
			unitCardSpecList.Add(UnitCardSpec.Create(specJson));
		}
		
		
		skillCardSpecList = new();
		deserializedObject = GameDataSystem.Instance.GameDataParams["SkillCardData"];
		foreach (var specJson in deserializedObject)
		{
			skillCardSpecList.Add(SkillCardSpec.Create(specJson));
		}
	}

	public override void Dispose()
	{
	}

	public ICardSpec GetRandomUnitCardSpec()
	{
		return unitCardSpecList[Random.Range(0, unitCardSpecList.Count)];
	}
	
	//todo: fix?
	public ICardSpec GetSpecById(int id)
	{
		return unitCardSpecList[id];
	}

	//todo: 별도로 dit 만들어서
	public SkillCardSpec GetSkillCardSpecByName(string str)
	{
		return skillCardSpecList.Find(x => x.Name == str);
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
		return skillCardSpecList[id];
		
	}
}