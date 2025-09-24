using System.Collections.Generic;
using UnityEngine;

public class AddShieldSkillActionSpec : SkillCardActionSpec
{
	public float actionDuration;
	public GameObject fxPrefab;
	
	public override SkillCardActionBase CreateCardAction()
	{
		return new AddShieldSkillAction(this);
	}

	protected override void OnInitialize(Dictionary<string, object> param)
	{
		
	}
}