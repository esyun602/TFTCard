using System.Collections.Generic;
using UnityEngine;

public class SharpenSkillActionSpec : SkillCardActionSpec
{
	public float actionDuration;
	public GameObject fxPrefab;
	
	public override SkillCardActionBase CreateCardAction()
	{
		return new SharpenSkillAction(this);
	}

	protected override void OnInitialize(Dictionary<string, object> param)
	{
		
	}
}