using System;
using UnityEngine;

public class BattleStage : StageBase
{
	protected MapData mapData;
	protected IMap map;
	public IMap Map => map;
	public BattleMap BattleMap { get; private set; }
	public BattleStage(BattleStageSpec stageSpec) : base(stageSpec)
	{
		mapData = stageSpec.MapData;
	}

	protected override void OnLoad()
	{
		map = mapData.InstantiateMap();
		map.Load();
		if (map is not BattleMap battleMap)
		{
			Debug.LogError("Battle stage must have battle map");
			throw new ArgumentException();
		}

		BattleMap = battleMap;
	}
}