using System.Collections.Generic;
using UnityEngine;

public class StockSkillActionSpec : SkillCardActionSpec
{
	public float actionDuration;
	public GameObject fxPrefab;

	public override SkillCardActionBase CreateCardAction()
	{
		return new StockSkillAction(this);
	}

	protected override void OnInitialize(Dictionary<string, object> param)
	{
		
	}
}