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

	public override void Initialize(Dictionary<string, object> param)
	{
		
	}
}