using System.Collections.Generic;
using UnityEngine;

public class PrepareBombingActionSpec : SkillCardActionSpec
{
	public override SkillCardActionBase CreateCardAction()
	{
		return new PrepareBombingAction(this);
	}

	public override void Initialize(Dictionary<string, object> param)
	{
		
	}
}