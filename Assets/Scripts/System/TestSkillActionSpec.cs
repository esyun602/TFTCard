using System.Collections.Generic;
using UnityEngine;

public class TestSkillActionSpec : SkillCardActionSpec
{
	public float actionDuration;
	public GameObject fxPrefab;
	
	public override SkillCardActionBase CreateCardAction()
	{
		return new TestSkillAction(this);
	}

	public override void Initialize(Dictionary<string, object> param)
	{
		
	}
}