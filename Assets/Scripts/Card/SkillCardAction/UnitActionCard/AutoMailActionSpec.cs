using System.Collections.Generic;
using UnityEngine;

public class AutoMailActionSpec : SkillCardActionSpec
{
	public override SkillCardActionBase CreateCardAction()
	{
		return new AutoMailAction(this);
	}

	protected override void OnInitialize(Dictionary<string, object> param)
	{
	}
}