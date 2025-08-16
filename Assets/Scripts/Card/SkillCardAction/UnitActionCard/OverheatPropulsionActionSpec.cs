using System.Collections.Generic;
using UnityEngine;

public class OverheatPropulsionActionSpec : SkillCardActionSpec
{
	public float actionDuration;
	public GameObject fxPrefab;
	
	public override SkillCardActionBase CreateCardAction()
	{
		return new OverheatPropulsionAction(this);
	}

	public override void Initialize(Dictionary<string, object> param)
	{
		
	}
}