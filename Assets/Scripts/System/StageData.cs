
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class StageData : GameData
{
	[SerializeField] private List<StageSpec> stageSpecList;
	
	//todo: fix

	public TestStageSpec GetTestStageSpec()
	{
		return (TestStageSpec)stageSpecList.Find((m) => m is TestStageSpec);
	}
	
	public override void Initialize()
	{
	}

	public override void Dispose()
	{
	}
}