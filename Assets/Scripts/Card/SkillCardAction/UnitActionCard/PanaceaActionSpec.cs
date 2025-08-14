using System.Collections.Generic;
using UnityEngine;

public class PanaceaActionSpec : SkillCardActionSpec
{
	public override SkillCardActionBase CreateCardAction()
	{
		return new PanaceaAction(this);
	}

	public override void Initialize(Dictionary<string, object> param)
	{
		
	}
}