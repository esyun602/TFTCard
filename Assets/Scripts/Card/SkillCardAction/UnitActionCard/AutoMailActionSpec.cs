using System.Collections.Generic;
using UnityEngine;

public class AutoMailActionSpec : SkillCardActionSpec
{
	public override SkillCardActionBase CreateCardAction()
	{
		return new AutoMailAction(this);
	}

	public override void Initialize(Dictionary<string, object> param)
	{
	}
}