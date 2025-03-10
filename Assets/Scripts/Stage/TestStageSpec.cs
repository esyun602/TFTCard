using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TestStageSpec : StageSpec
{
	[SerializeField] private CardSpec testCard;
	public List<WaveGrid> WaveData;
	
	public override IStage InstantiateStage()
	{
		return new TestStage(testCard, this);
	}
}