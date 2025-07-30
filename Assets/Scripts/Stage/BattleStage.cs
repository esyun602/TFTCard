using System;
using UnityEngine;

public class BattleStage : StageBase
{
	public BattleMap BattleMap { get; private set; }
	public BattleStage(StageSpec stageSpec) : base(stageSpec)
	{
	}

	protected override void OnLoad()
	{
		if (map is not BattleMap battleMap)
		{
			Debug.LogError("Battle stage must have battle map");
			throw new ArgumentException();
		}

		BattleMap = battleMap;
	}
}