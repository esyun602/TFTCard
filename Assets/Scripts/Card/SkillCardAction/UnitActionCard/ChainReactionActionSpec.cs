using System.Collections.Generic;
using UnityEngine;

public class ChainReactionActionSpec : SkillCardActionSpec
{
	public override SkillCardActionBase CreateCardAction()
	{
		return new ChainReactionAction(this);
	}

	public override void Initialize(Dictionary<string, object> param)
	{
		
	}
}