using System.Collections.Generic;
using UnityEngine;

public class PanaceaActionSpec : SkillCardActionSpec
{
	public override SkillCardActionBase CreateCardAction()
	{
		return new PanaceaAction(this);
	}

	protected override void OnInitialize(Dictionary<string, object> param)
	{
		
	}
}