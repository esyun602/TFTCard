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

	public override void Initialize(Dictionary<string, object> param)
	{
		
	}
}