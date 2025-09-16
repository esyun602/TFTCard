using System.Collections.Generic;
using UnityEngine;

public class BreadSupplySkillActionSpec : SkillCardActionSpec
{
	public float actionDuration;
	public GameObject fxPrefab;
	public override SkillCardActionBase CreateCardAction()
	{
		return new BreadSupplySkillAction(this);
	}

	protected override void OnInitialize(Dictionary<string, object> param)
	{
		
	}
}