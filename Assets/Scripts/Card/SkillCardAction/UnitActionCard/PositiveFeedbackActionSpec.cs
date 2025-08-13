using System.Collections.Generic;
using UnityEngine;

public class PositiveFeedbackActionSpec : SkillCardActionSpec
{
	public override SkillCardActionBase CreateCardAction()
	{
		return new PositiveFeedbackAction(this);
	}

	public override void Initialize(Dictionary<string, object> param)
	{
		
	}
}