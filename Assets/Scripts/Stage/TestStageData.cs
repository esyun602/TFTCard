
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TestStageData : StageData
{
	[SerializeField] private CardData testCard;
	public List<WaveGrid> WaveData;
	
	public override IStage InstantiateStage()
	{
		return new TestStage(testCard, this);
	}
}