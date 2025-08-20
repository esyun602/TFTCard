using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class StageData : GameData
{
	private Dictionary<string, StageSpec> stageSpecDict;

	//todo: fix

	public TestStageSpec GetTestStageSpec()
	{
		return (TestStageSpec)stageSpecDict["TestStage"];
	}
	
	public StageSpec GetStageSpec(string name)
	{
		return stageSpecDict[name];
	}

	public override void Initialize()
	{
		stageSpecDict = new();
		var stageParams = GameDataSystem.Instance.GameDataParams["StageData"];
		foreach (var param in stageParams)
		{
			var spec = StageSpec.Create(param);
			stageSpecDict[spec.StageName] = spec;
		}
	}

	public override void Dispose()
	{
	}
}