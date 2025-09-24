using System.Collections.Generic;
using UnityEngine;

public class SpringHookActionSpec : SkillCardActionSpec
{
	public override SkillCardActionBase CreateCardAction()
	{
		return new SpringHookAction(this);
	}

	protected override void OnInitialize(Dictionary<string, object> param)
	{
		
	}
}