using System.Collections.Generic;
using UnityEngine;

public class UnitCardRangeAttackActionSpec : UnitCardActionSpec
{
	public float actionDuration;

	public GameObject fxPrefab;

	//public GridSelector actionRange;
	public override UnitCardActionBase CreateCardAction()
	{
		return new UnitCardRangeAttackAction(this);
	}

	protected override void OnInitialize(Dictionary<string, object> param)
	{
		fxPrefab = Resources.Load<GameObject>("Fx/" + param.GetString("FxName"));
	}
	//todo: 추후 툴 개발
}