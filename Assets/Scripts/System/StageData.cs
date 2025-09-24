using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

public class StageData : GameData
{
	private Dictionary<string, StageSpec> stageSpecDict;
	private Dictionary<StageType, List<StageSpec>> stageTypeToSpecDict;

	//todo: fix

	public BattleStageSpec GetTestStageSpec()
	{
		return (BattleStageSpec)stageSpecDict["TestStage"];
	}
	
	public StageSpec GetStageSpec(string name)
	{
		return stageSpecDict[name];
	}

	public override void Initialize()
	{
		stageTypeToSpecDict = new();
		stageSpecDict = new();
		var stageParams = GameDataSystem.Instance.GameDataParams["StageData"];
		foreach (var param in stageParams)
		{
			var spec = StageSpec.Create(param);
			if (stageTypeToSpecDict.TryGetValue(spec.StageType, out var ls))
			{
				ls.Add(spec);
			}
			else
			{
				stageTypeToSpecDict[spec.StageType] = new List<StageSpec>() { spec };
			}
			stageSpecDict[spec.StageName] = spec;
		}
	}

	public StageSpec GetRandomStageWithType(StageType type)
	{
		return stageTypeToSpecDict[type].GetRandomElement();
	}

	public override void Dispose()
	{
	}
}