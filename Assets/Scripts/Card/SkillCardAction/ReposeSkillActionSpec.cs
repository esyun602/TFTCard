using System.Collections.Generic;
using UnityEngine;

public class ReposeSkillActionSpec : SkillCardActionSpec
{
	public float actionDuration;
	public GameObject fxPrefab;
	public override SkillCardActionBase CreateCardAction()
	{
		return new ReposeSkillAction(this);
	}

	public override void Initialize(Dictionary<string, object> param)
	{
		
	}
}