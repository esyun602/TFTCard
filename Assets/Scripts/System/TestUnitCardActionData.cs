using UnityEngine;

[CreateAssetMenu]
public class TestUnitCardActionData : UnitCardActionData
{
	public float actionDuration;
	public GameObject fxPrefab;
	public GridSelector actionRange;
	public override UnitCardActionBase CreateCardAction()
	{
		return new TestUnitCardAction(this);
	}
	//todo: 추후 툴 개발
}