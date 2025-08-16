using System.Collections.Generic;
using UnityEngine;

public class EquivalentExchangeActionSpec : SkillCardActionSpec
{
	public override SkillCardActionBase CreateCardAction()
	{
		return new EquivalentExchangeAction(this);
	}

	public override void Initialize(Dictionary<string, object> param)
	{
		
	}
}