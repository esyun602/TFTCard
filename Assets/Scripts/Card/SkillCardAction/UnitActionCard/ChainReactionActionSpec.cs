using System.Collections.Generic;
using UnityEngine;

public class ChainReactionActionSpec : SkillCardActionSpec
{
	public override SkillCardActionBase CreateCardAction()
	{
		return new ChainReactionAction(this);
	}

	protected override void OnInitialize(Dictionary<string, object> param)
	{
		
	}
}