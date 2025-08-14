using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class ActionData : GameData
{
	private Dictionary<string, SkillCardActionSpec> skillActionSpecDict;
	private Dictionary<string, UnitCardActionSpec> unitActionSpecDict;

	public SkillCardActionSpec GetSkillActionByName(string name)
	{
		return skillActionSpecDict.GetValueOrDefault(name);
	}

	public UnitCardActionSpec GetUnitActionByName(string name)
	{
		return unitActionSpecDict.GetValueOrDefault(name);
	}

	public override void Initialize()
	{
		skillActionSpecDict = new();
		var actionParam = GameDataSystem.Instance.GameDataParams["SkillActionData"];
		foreach (var param in actionParam)
		{
			var className = param.GetString("ClassName") + "Spec";
			var type = Type.GetType(className);
			var spec = (SkillCardActionSpec)Activator.CreateInstance(type ?? throw new InvalidOperationException());

			skillActionSpecDict[param.GetString("Name")] = spec;
			spec.Initialize(param);
		}


		unitActionSpecDict = new();
		actionParam = GameDataSystem.Instance.GameDataParams["UnitActionData"];
		foreach (var param in actionParam)
		{
			var className = param.GetString("ClassName") + "Spec";
			var type = Type.GetType(className);
			var spec = (UnitCardActionSpec)Activator.CreateInstance(type ?? throw new InvalidOperationException());

			unitActionSpecDict[param.GetString("Name")] = spec;
			spec.Initialize(param);
		}
	}

	public override void Dispose()
	{
	}
}