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

	public override void Initialize(Dictionary<string, object> param)
	{
		
	}
}