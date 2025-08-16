using System.Collections.Generic;
using UnityEngine;

public class HighPressureBombActionSpec : SkillCardActionSpec
{
	public override SkillCardActionBase CreateCardAction()
	{
		return new HighPressureBombAction(this);
	}

	public override void Initialize(Dictionary<string, object> param)
	{
		
	}
}