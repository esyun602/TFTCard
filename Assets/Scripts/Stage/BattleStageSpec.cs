using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

//todo: 나중에 이걸 배틀로
public class BattleStageSpec : StageSpec
{
	public List<string> WaveGridList { get; private set; }
	public MapData MapData { get; private set; }
	
	public override IStage InstantiateStage()
	{
		return new BattleStage(this);
	}

	public override StageType StageType => StageType.BattleStage;

	protected override void Initialize(Dictionary<string, object> param)
	{
		WaveGridList = new List<string>(param.GetStringArray(nameof(WaveGridList)));
		var mapName = param.GetString(nameof(MapData));
		MapData = Resources.Load<MapData>("Map/" + mapName);
	}
}