using System.Collections.Generic;
using UnityEngine;

public class SpringHookActionSpec : SkillCardActionSpec
{
	public override SkillCardActionBase CreateCardAction()
	{
		return new SpringHookAction(this);
	}

	public override void Initialize(Dictionary<string, object> param)
	{
		
	}
}