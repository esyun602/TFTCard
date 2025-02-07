
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TestStageData : StageData
{
	[SerializeField] private CardData testCard;
	public List<WaveData> WaveDataList;
	
	public override IStage InstantiateStage()
	{
		return new TestStage(testCard, this);
	}
}