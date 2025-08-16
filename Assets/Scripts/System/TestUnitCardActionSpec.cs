using System.Collections.Generic;
using UnityEngine;

public class TestUnitCardActionSpec : UnitCardActionSpec
{
	public float actionDuration;
	public GameObject fxPrefab;
	//public GridSelector actionRange;
	public override UnitCardActionBase CreateCardAction()
	{
		return new TestUnitCardAction(this);
	}

	public override void Initialize(Dictionary<string, object> param)
	{
		
	}
	//todo: 추후 툴 개발
}