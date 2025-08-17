using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

//todo: 나중에 이걸 배틀로
public class TestStageSpec : StageSpec
{
	public List<string> WaveGridList { get; private set; }
	
	public override IStage InstantiateStage()
	{
		return new TestStage(this);
	}

	protected override void Initialize(Dictionary<string, object> param)
	{
		WaveGridList = new List<string>(param.GetStringArray(nameof(WaveGridList)));
		var mapName = param.GetString(nameof(MapData));
	}
}