using System.Collections.Generic;
using UnityEngine;

public class HighPressureBombActionSpec : SkillCardActionSpec
{
	public override SkillCardActionBase CreateCardAction()
	{
		return new HighPressureBombAction(this);
	}

	protected override void OnInitialize(Dictionary<string, object> param)
	{
		
	}
}