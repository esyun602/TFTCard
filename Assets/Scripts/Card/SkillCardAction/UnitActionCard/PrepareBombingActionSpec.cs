using System.Collections.Generic;
using UnityEngine;

public class PrepareBombingActionSpec : SkillCardActionSpec
{
	public override SkillCardActionBase CreateCardAction()
	{
		return new PrepareBombingAction(this);
	}

	protected override void OnInitialize(Dictionary<string, object> param)
	{
		
	}
}