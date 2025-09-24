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

	protected override void OnInitialize(Dictionary<string, object> param)
	{
		fxPrefab = Resources.Load<GameObject>("Fx/" + param.GetString("FxName"));
	}
	//todo: 추후 툴 개발
}