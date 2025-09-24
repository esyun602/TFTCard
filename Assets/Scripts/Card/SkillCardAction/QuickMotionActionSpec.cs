using System.Collections.Generic;
using UnityEngine;

public class QuickMotionActionSpec : SkillCardActionSpec
{
	public float actionDuration;
	public GameObject fxPrefab;
	
	public override SkillCardActionBase CreateCardAction()
	{
		return new QuickMotionAction(this);
	}

	protected override void OnInitialize(Dictionary<string, object> param)
	{
		
	}
}