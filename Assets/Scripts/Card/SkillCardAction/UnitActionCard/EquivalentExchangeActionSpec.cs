using System.Collections.Generic;
using UnityEngine;

public class EquivalentExchangeActionSpec : SkillCardActionSpec
{
	public override SkillCardActionBase CreateCardAction()
	{
		return new EquivalentExchangeAction(this);
	}

	protected override void OnInitialize(Dictionary<string, object> param)
	{
		
	}
}