using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class TestStageSpec : StageSpec
{
	[FormerlySerializedAs("testCard")] [SerializeField] private UnitCardSpec testUnitCard;
	public List<WaveGrid> WaveData;
	
	public override IStage InstantiateStage()
	{
		return new TestStage(testUnitCard, this);
	}
}