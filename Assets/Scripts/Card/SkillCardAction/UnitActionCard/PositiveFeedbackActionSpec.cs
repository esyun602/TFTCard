using System.Collections.Generic;
using UnityEngine;

public class PositiveFeedbackActionSpec : SkillCardActionSpec
{
	public override SkillCardActionBase CreateCardAction()
	{
		return new PositiveFeedbackAction(this);
	}

	protected override void OnInitialize(Dictionary<string, object> param)
	{
		
	}
}